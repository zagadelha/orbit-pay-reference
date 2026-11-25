using Confluent.Kafka;
using OrbitPay.OutboxRelay;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerClient("orbitdb");

builder.AddKafkaProducer<string, string>("kafka", settings =>
{
    settings.Config.EnableIdempotence = true;   // sem duplicatas nem reordenação por falha de rede
    settings.Config.Acks = Acks.All;
    settings.Config.LingerMs = 5;               // agrupa mensagens em lotes pequenos
});

builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddSource(OutboxRelayWorker.TelemetryName))
    .WithMetrics(m => m.AddMeter(OutboxRelayWorker.TelemetryName));

builder.Services.AddHostedService<OutboxRelayWorker>();

builder.Build().Run();