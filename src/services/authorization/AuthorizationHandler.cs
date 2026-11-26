using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;

namespace OrbitPay.Authorization;

public sealed class AuthorizationHandler(SqlConnection connection, NetworkClient network)
{
      public const string TelemetryName = "OrbitPay.Authorization";
    private const decimal StandInLimit = 50m;

    private static readonly Meter Meter = new(TelemetryName);
    private static readonly Counter<long> Authorizations = Meter.CreateCounter<long>("orbit.authorizations");
    private static readonly Counter<long> StandInApprovals = Meter.CreateCounter<long>("orbit.stip.approvals");
    
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<IResult> HandleAsync(string idempotencyKey, AuthorizationRequest request, CancellationToken ct)
    {
        var requestHash = SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request, Json)));

        await connection.OpenAsync(ct);
        await using var tx = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

        // 1. Reserva a chave de idempotência DENTRO da transação.
        //    Uma requisição duplicada concorrente fica bloqueada até esta terminar
        //    e então recebe violação de chave, devolvendo a resposta já gravada.
        try
        {
            await connection.ExecuteAsync(new CommandDefinition(
                """
                INSERT INTO idempotency_keys (idempotency_key, request_hash, expires_at)
                VALUES (@Key, @Hash, DATEADD(HOUR, 24, SYSUTCDATETIME()))
                """,
                new { Key = Ansi(idempotencyKey, 100), Hash = requestHash }, tx, cancellationToken: ct));
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            await tx.RollbackAsync(ct);
            return await ReplayAsync(idempotencyKey, requestHash, ct);
        }

        // 2. Cartão e conta
        var card = await connection.QuerySingleOrDefaultAsync<CardRow>(new CommandDefinition(
            "SELECT account_id AS AccountId, status AS Status FROM cards WHERE card_token = @CardToken",
            new { CardToken = Ansi(request.CardToken, 40) }, tx, cancellationToken: ct));

        string status;
        string? declineReason = null;
        var standIn = false;
        var accountId = card?.AccountId ?? 0;

        if (card is null)
        {
            status = "DECLINED"; declineReason = "CARD_NOT_FOUND";
        }
        else if (card.Status != "ACTIVE")
        {
            status = "DECLINED"; declineReason = "CARD_BLOCKED";
        }
        else
        {
            var decision = await network.AuthorizeAsync(idempotencyKey, request, ct);

            if (decision == NetworkDecision.Declined)
            {
                status = "DECLINED"; declineReason = "NETWORK_DECLINED";
            }
            else if (decision == NetworkDecision.Unavailable && request.Amount >= StandInLimit)
            {
                status = "DECLINED"; declineReason = "NETWORK_UNAVAILABLE";
            }
            else
            {
                // Aprovada pela rede, ou pelo STIP quando a rede está fora e o valor é baixo
                standIn = decision == NetworkDecision.Unavailable;

                var rows = await connection.ExecuteAsync(new CommandDefinition(
                    """
                    UPDATE accounts
                       SET available = available - @Amount, updated_at = SYSUTCDATETIME()
                     WHERE account_id = @AccountId AND available >= @Amount
                    """,
                    new { request.Amount, AccountId = accountId }, tx, cancellationToken: ct));

                (status, declineReason) = rows == 1 ? ("APPROVED", null) : ("DECLINED", "INSUFFICIENT_FUNDS");
                if (status == "DECLINED") standIn = false;
            }
        }

        // 4. Registro da autorização (aprovadas e recusadas)
        var auth = await connection.QuerySingleAsync<AuthRow>(new CommandDefinition(
            """
            INSERT INTO authorizations (card_token, account_id, amount, currency, merchant_id, mcc, status, decline_reason, stand_in)
            OUTPUT INSERTED.authorization_id AS AuthorizationId, INSERTED.created_at AS CreatedAt
            VALUES (@CardToken, @AccountId, @Amount, @Currency, @MerchantId, @Mcc, @Status, @DeclineReason, @StandIn)
            """,
            new { request.CardToken, AccountId = accountId, request.Amount, request.Currency,
                  request.MerchantId, request.Mcc, Status = status, DeclineReason = declineReason, StandIn = standIn },
            tx, cancellationToken: ct));

        var response = new AuthorizationResponse(auth.AuthorizationId, status, declineReason,
                                                 request.Amount, request.Currency, auth.CreatedAt);
        var responseJson = JsonSerializer.Serialize(response, Json);

        // 5. Evento na outbox, na MESMA transação
        var evt = new
        {
            eventId = Guid.NewGuid(),
            type = status == "APPROVED" ? "AuthorizationApproved" : "AuthorizationDeclined",
            version = 1,
            occurredAt = auth.CreatedAt,
            authorizationId = auth.AuthorizationId,
            cardToken = request.CardToken,
            accountId,
            amount = request.Amount,
            currency = request.Currency,
            merchantId = request.MerchantId,
            mcc = request.Mcc,
            declineReason
        };

        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO outbox (event_id, partition_key, event_type, payload, traceparent)
            VALUES (@EventId, @PartitionKey, @EventType, @Payload, @TraceParent)
            """,
            new { EventId = evt.eventId, PartitionKey = request.CardToken, EventType = evt.type,
                  Payload = JsonSerializer.Serialize(evt, Json), TraceParent = Activity.Current?.Id },
            tx, cancellationToken: ct));

        // 6. Guarda a resposta para reenvios com a mesma chave
        await connection.ExecuteAsync(new CommandDefinition(
            "UPDATE idempotency_keys SET status_code = 200, response_body = @Body WHERE idempotency_key = @Key",
            new { Body = responseJson, Key = Ansi(idempotencyKey, 100) }, tx, cancellationToken: ct));

        Authorizations.Add(1, new KeyValuePair<string, object?>("result", status),
                              new KeyValuePair<string, object?>("reason", declineReason ?? "none"));
        if (standIn) StandInApprovals.Add(1);

        await tx.CommitAsync(ct);
        return Results.Text(responseJson, "application/json", Encoding.UTF8, StatusCodes.Status200OK);
    }

    private async Task<IResult> ReplayAsync(string key, byte[] requestHash, CancellationToken ct)
    {
        var row = await connection.QuerySingleOrDefaultAsync<IdempotencyRow>(new CommandDefinition(
            """
            SELECT request_hash AS RequestHash, status_code AS StatusCode, response_body AS ResponseBody
              FROM idempotency_keys WHERE idempotency_key = @Key
            """,
            new { Key = Ansi(key, 100) }, cancellationToken: ct));

        if (row is null || row.StatusCode is null)
            return Results.Problem(statusCode: StatusCodes.Status409Conflict,
                                   title: "Requisição com esta chave ainda em processamento. Tente novamente.");

        if (!row.RequestHash.AsSpan().SequenceEqual(requestHash))
            return Results.Problem(statusCode: StatusCodes.Status422UnprocessableEntity,
                                   title: "Idempotency-Key reutilizada com um corpo diferente.");

        return Results.Text(row.ResponseBody!, "application/json", Encoding.UTF8, row.StatusCode.Value);
    }

    // Parâmetros para colunas VARCHAR: evita conversão implícita, varredura e deadlock.
    private static DbString Ansi(string value, int length) =>
        new() { Value = value, IsAnsi = true, Length = length };

    private sealed record CardRow(long AccountId, string Status);
    private sealed record AuthRow(Guid AuthorizationId, DateTime CreatedAt);
    private sealed record IdempotencyRow(byte[] RequestHash, int? StatusCode, string? ResponseBody);
}