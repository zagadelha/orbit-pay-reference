using Polly.CircuitBreaker;
using Polly.Timeout;

namespace OrbitPay.Authorization;

public enum NetworkDecision { Approved, Declined, Unavailable }

public sealed class NetworkClient(HttpClient http, ILogger<NetworkClient> logger)
{
    public async Task<NetworkDecision> AuthorizeAsync(string idempotencyKey, AuthorizationRequest request, CancellationToken ct)
    {
        try
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, "/authorize")
            {
                Content = JsonContent.Create(new { request.CardToken, request.Amount, request.Currency, request.Mcc })
            };
            // A rede também precisa de idempotência, porque o Polly pode repetir a chamada.
            message.Headers.Add("Idempotency-Key", idempotencyKey);

            using var response = await http.SendAsync(message, ct);
            if (!response.IsSuccessStatusCode)
                return NetworkDecision.Unavailable;

            var body = await response.Content.ReadFromJsonAsync<NetworkResponse>(ct);
            return body?.Approved == true ? NetworkDecision.Approved : NetworkDecision.Declined;
        }
        catch (Exception ex) when (ex is BrokenCircuitException or TimeoutRejectedException or HttpRequestException)
        {
            logger.LogWarning("Rede de cartões indisponível: {Reason}", ex.GetType().Name);
            return NetworkDecision.Unavailable;
        }
    }

    private sealed record NetworkResponse(bool Approved, string NetworkRef);
}