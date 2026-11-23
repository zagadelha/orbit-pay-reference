var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithDataVolume()
                 .AddDatabase("orbitdb");

var redis = builder.AddRedis("redis");

var kafka = builder.AddKafka("kafka")
                   .WithKafkaUI();

builder.AddDotnetProject("authorization", "../src/services/authorization/OrbitPay.Authorization.csproj")
       .WithReference(sql)
       .WithReference(redis)
       .WithReference(kafka)
       .WaitFor(sql)
       .WaitFor(kafka);

builder.Build().Run();