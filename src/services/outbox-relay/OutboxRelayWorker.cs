using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Dapper;
using Microsoft.Data.SqlClient;

namespace OrbitPay.OutboxRelay;

public sealed class OutboxRelayWorker(
    IServiceScopeFactory scopeFactory,
    IProducer<string, string> producer,
    IConfiguration configuration,
    ILogger<OutboxRelayWorker> logger) : BackgroundService
{
    public const string Topic = "card-transactions";
    public const string TelemetryName = "OrbitPay.OutboxRelay";
    private const int BatchSize = 500;

    private static readonly ActivitySource Source = new(TelemetryName);
    private static readonly Meter Meter = new(TelemetryName);
    private static readonly Counter<long> Published = Meter.CreateCounter<long>("orbit.outbox.published");
    private static readonly Histogram<double> Lag = Meter.CreateHistogram<double>("orbit.outbox.lag", unit: "ms",
        description: "Tempo entre o commit da autorização e a publicação no Kafka");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await EnsureTopicAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var count = await PublishBatchAsync(stoppingToken);

                // Lote cheio: provavelmente há mais pendentes, continua sem pausa.
                if (count < BatchSize)
                    await Task.Delay(TimeSpan.FromMilliseconds(200), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao publicar lote da outbox. Nova tentativa em 2 s.");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }

    private async Task<int> PublishBatchAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var connection = scope.ServiceProvider.GetRequiredService<SqlConnection>();

        await connection.OpenAsync(ct);
        await using var tx = (SqlTransaction)await connection.BeginTransactionAsync(ct);

        var rows = (await connection.QueryAsync<OutboxRow>(new CommandDefinition(
            """
            SELECT TOP (@BatchSize)
                   id AS Id, event_id AS EventId, partition_key AS PartitionKey,
                   event_type AS EventType, payload AS Payload,
                   traceparent AS TraceParent, created_at AS CreatedAt
              FROM outbox WITH (UPDLOCK, READPAST, ROWLOCK)
             WHERE published_at IS NULL
             ORDER BY id
            """,
            new { BatchSize }, tx, cancellationToken: ct))).AsList();

        if (rows.Count == 0)
        {
            await tx.CommitAsync(ct);
            return 0;
        }

        // Publica em paralelo; o produtor idempotente preserva a ordem por partição.
        await Task.WhenAll(rows.Select(row => PublishAsync(row, ct)));

        await connection.ExecuteAsync(new CommandDefinition(
            "UPDATE outbox SET published_at = SYSUTCDATETIME() WHERE id IN @Ids",
            new { Ids = rows.Select(r => r.Id) }, tx, cancellationToken: ct));

        await tx.CommitAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var row in rows)
            Lag.Record((now - row.CreatedAt).TotalMilliseconds);
        Published.Add(rows.Count);

        return rows.Count;
    }

    private async Task PublishAsync(OutboxRow row, CancellationToken ct)
    {
        // Continua o trace da requisição original que gerou o evento.
        ActivityContext.TryParse(row.TraceParent, null, out var parent);
        using var activity = Source.StartActivity($"{Topic} publish", ActivityKind.Producer, parent);
        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination.name", Topic);
        activity?.SetTag("messaging.message.id", row.EventId.ToString());

        var headers = new Headers
        {
            { "event-id", Encoding.UTF8.GetBytes(row.EventId.ToString()) },
            { "event-type", Encoding.UTF8.GetBytes(row.EventType) }
        };

        // O consumidor vai continuar o trace a partir deste span.
        var traceparent = activity?.Id ?? row.TraceParent;
        if (traceparent is not null)
            headers.Add("traceparent", Encoding.UTF8.GetBytes(traceparent));

        await producer.ProduceAsync(Topic, new Message<string, string>
        {
            Key = row.PartitionKey,
            Value = row.Payload,
            Headers = headers
        }, ct);
    }

    private async Task EnsureTopicAsync()
    {
        var bootstrap = configuration.GetConnectionString("kafka");
        using var admin = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrap }).Build();

        try
        {
            await admin.CreateTopicsAsync(
            [
                new TopicSpecification { Name = Topic, NumPartitions = 8, ReplicationFactor = 1 }
            ]);
            logger.LogInformation("Tópico {Topic} criado com 8 partições.", Topic);
        }
        catch (CreateTopicsException ex) when (ex.Results.All(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
        {
            logger.LogInformation("Tópico {Topic} já existe.", Topic);
        }
    }

    private sealed record OutboxRow(
        long Id, Guid EventId, string PartitionKey, string EventType,
        string Payload, string? TraceParent, DateTime CreatedAt);
}