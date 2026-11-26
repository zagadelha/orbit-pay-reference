var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSingleton<FaultSettings>();

var app = builder.Build();
app.MapDefaultEndpoints();

// Autorização na "rede": latência e erros configuráveis em tempo real
app.MapPost("/authorize", async (NetworkAuthorizeRequest request, FaultSettings faults, CancellationToken ct) =>
{
    if (faults.LatencyMs > 0)
        await Task.Delay(faults.LatencyMs, ct);

    if (Random.Shared.NextDouble() < faults.ErrorRate)
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);

    var approved = request.Amount <= 5000m;   // regra simples da rede
    return Results.Ok(new { approved, networkRef = $"nw_{Guid.NewGuid():N}"[..16] });
});

// Painel de controle do chaos
app.MapGet("/admin/faults", (FaultSettings f) => Results.Ok(new { f.LatencyMs, f.ErrorRate }));

app.MapPost("/admin/faults", (FaultUpdate update, FaultSettings f) =>
{
    f.LatencyMs = Math.Clamp(update.LatencyMs, 0, 10_000);
    f.ErrorRate = Math.Clamp(update.ErrorRate, 0, 1);
    return Results.Ok(new { f.LatencyMs, f.ErrorRate });
});

app.Run();

record NetworkAuthorizeRequest(string CardToken, decimal Amount, string Currency, string Mcc);
record FaultUpdate(int LatencyMs, double ErrorRate);

sealed class FaultSettings
{
    public int LatencyMs { get; set; } = 40;   // latência "normal" de rede
    public double ErrorRate { get; set; }
}