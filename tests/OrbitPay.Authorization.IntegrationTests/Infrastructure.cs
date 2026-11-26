using System.Net;
using System.Net.Http.Json;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using OrbitPay.Authorization;
using OrbitPay.Migrations;
using Testcontainers.MsSql;

namespace OrbitPay.Authorization.IntegrationTests;

/// <summary>Um SQL Server real em container, compartilhado por todos os testes.</summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        ConnectionString = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "orbitdb_tests"
        }.ConnectionString;

        var result = DatabaseMigrator.Run(ConnectionString, logToConsole: false);
        if (!result.Successful) throw result.Error;

        // Configuração lida pela API no momento em que ela sobe
        Environment.SetEnvironmentVariable("ConnectionStrings__orbitdb", ConnectionString);
        Environment.SetEnvironmentVariable("services__network-sim__https__0", "https://network-sim.test");
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}

[CollectionDefinition("sql")]
public sealed class SqlCollection : ICollectionFixture<SqlServerFixture>;

/// <summary>Rede de cartões falsa: responde aprovado, ou 503 quando "derrubada".</summary>
public sealed class FakeNetworkHandler : HttpMessageHandler
{
    public volatile bool Down;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (Down)
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new { approved = true, networkRef = "nw_test" })
        });
    }
}

/// Sobe a API em memória, trocando só o transporte da rede de cartões.</summary>
/// O pipeline Polly continua ativo, então timeout, retry e circuit breaker são testados de verdade.
public sealed class AuthorizationApiFactory(FakeNetworkHandler network) : WebApplicationFactory<AuthorizationHandler>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
            services.AddHttpClient<NetworkClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => network));
    }
}

public static class TestData
{
    /// <summary>Cria uma conta e um cartão exclusivos para o teste.</summary>
    public static async Task<(long AccountId, string CardToken)> NewCardAsync(string connectionString, decimal limit)
    {
        var accountId = Random.Shared.NextInt64(1_000_000, long.MaxValue);
        var cardToken = $"tok_test_{Guid.NewGuid():N}"[..30];

        await using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(
            """
            INSERT INTO accounts (account_id, customer_id, credit_limit, available) VALUES (@Id, @Id, @Limit, @Limit);
            INSERT INTO cards (card_token, account_id) VALUES (@Token, @Id);
            """,
            new { Id = accountId, Limit = limit, Token = cardToken });

        return (accountId, cardToken);
    }
}

public static class Api
{
    public static Task<HttpResponseMessage> AuthorizeAsync(HttpClient client, string key, string cardToken, decimal amount)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/authorizations")
        {
            Content = JsonContent.Create(new
            {
                cardToken, amount, currency = "EUR", merchantId = "m_test", mcc = "5411"
            })
        };
        request.Headers.Add("Idempotency-Key", key);
        return client.SendAsync(request);
    }
}