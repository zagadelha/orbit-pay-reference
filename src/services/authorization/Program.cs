using Microsoft.Extensions.Http.Resilience;
using Polly;
using Microsoft.AspNetCore.Mvc;
using OrbitPay.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerClient("orbitdb");

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddScoped<AuthorizationHandler>();

builder.Services.AddHttpClient<NetworkClient>(client =>
        client.BaseAddress = new Uri("https+http://network-sim"))   // service discovery do Aspire
    .AddResilienceHandler("card-network", pipeline =>
    {
        // Ordem: de fora para dentro
        pipeline.AddTimeout(TimeSpan.FromMilliseconds(800));          // orçamento total

        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 2,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            Delay = TimeSpan.FromMilliseconds(50)
        });

        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,                              // 50% de falhas...
            SamplingDuration = TimeSpan.FromSeconds(10),     // ...em 10 s...
            MinimumThroughput = 20,                          // ...com pelo menos 20 chamadas
            BreakDuration = TimeSpan.FromSeconds(15)
        });

        pipeline.AddTimeout(TimeSpan.FromMilliseconds(300));          // por tentativa
    });

builder.Services.AddOpenTelemetry().WithMetrics(m => m.AddMeter("Polly", AuthorizationHandler.TelemetryName));

var app = builder.Build();

app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/v1/authorizations", async (
    [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
    AuthorizationRequest request,
    AuthorizationHandler handler,
    CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Length > 100)
        return Results.Problem(statusCode: 400, title: "Header Idempotency-Key obrigatório (até 100 caracteres).");

    if (string.IsNullOrWhiteSpace(request.CardToken) || request.Amount <= 0 ||
        request.Currency?.Length != 3 || request.Mcc?.Length != 4 ||
        string.IsNullOrWhiteSpace(request.MerchantId))
        return Results.Problem(statusCode: 400, title: "Requisição inválida.");

    return await handler.HandleAsync(idempotencyKey, request, ct);
});

app.Run();