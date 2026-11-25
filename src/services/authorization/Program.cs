using Microsoft.AspNetCore.Mvc;
using OrbitPay.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerClient("orbitdb");

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddScoped<AuthorizationHandler>();

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