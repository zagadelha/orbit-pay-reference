var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithDataVolume()
                 .AddDatabase("orbitdb");

var redis = builder.AddRedis("redis");

var kafka = builder.AddKafka("kafka")
                   .WithKafkaUI();

var migrations = builder.AddDotnetProject("migrations", "../src/migrations/OrbitPay.Migrations.csproj")
                        .WithReference(sql)
                        .WaitFor(sql);

var networkSim = builder.AddDotnetProject("network-sim", "../src/simulators/network-sim/OrbitPay.NetworkSim.csproj");

builder.AddDotnetProject("authorization", "../src/services/authorization/OrbitPay.Authorization.csproj")
       .WithReference(sql)
       .WithReference(redis)
       .WithReference(kafka)
       .WithReference(networkSim)
       .WaitForCompletion(migrations)
       .WaitFor(kafka)
       .WaitFor(networkSim);

builder.AddDotnetProject("outbox-relay", "../src/services/outbox-relay/OrbitPay.OutboxRelay.csproj")
       .WithReference(sql)
       .WithReference(kafka)
       .WaitForCompletion(migrations)
       .WaitFor(kafka);

builder.Build().Run();