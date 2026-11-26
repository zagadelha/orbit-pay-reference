using System.Net;
using System.Net.Http.Json;
using Dapper;
using Microsoft.Data.SqlClient;

namespace OrbitPay.Authorization.IntegrationTests;

[Collection("sql")]
public sealed class ConcurrencyTests(SqlServerFixture db)
{
    [Fact]
    public async Task Cem_requisicoes_paralelas_com_a_mesma_chave_geram_uma_unica_autorizacao()
    {
        var (accountId, token) = await TestData.NewCardAsync(db.ConnectionString, limit: 1000m);
        await using var factory = new AuthorizationApiFactory(new FakeNetworkHandler());
        var client = factory.CreateClient();
        var key = $"idem-{Guid.NewGuid():N}";

        var responses = await Task.WhenAll(
            Enumerable.Range(0, 100).Select(_ => Api.AuthorizeAsync(client, key, token, 10m)));

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));

        var ids = await Task.WhenAll(responses.Select(async r =>
            (await r.Content.ReadFromJsonAsync<AuthorizationResponse>())!.AuthorizationId));
        Assert.Single(ids.Distinct());

        await using var connection = new SqlConnection(db.ConnectionString);
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM authorizations WHERE card_token = @token", new { token }));
        Assert.Equal(990m, await connection.ExecuteScalarAsync<decimal>(
            "SELECT available FROM accounts WHERE account_id = @accountId", new { accountId }));
    }

    [Fact]
    public async Task Duzentas_compras_paralelas_nunca_deixam_o_saldo_negativo()
    {
        var (accountId, token) = await TestData.NewCardAsync(db.ConnectionString, limit: 1000m);
        await using var factory = new AuthorizationApiFactory(new FakeNetworkHandler());
        var client = factory.CreateClient();

        // 200 compras de 10 € contra 1.000 € de limite: só cabem 100
        var responses = await Task.WhenAll(
            Enumerable.Range(0, 200).Select(i => Api.AuthorizeAsync(client, $"race-{Guid.NewGuid():N}", token, 10m)));

        var bodies = await Task.WhenAll(responses.Select(r =>
            r.Content.ReadFromJsonAsync<AuthorizationResponse>()));

        Assert.Equal(100, bodies.Count(b => b!.Status == "APPROVED"));
        Assert.Equal(100, bodies.Count(b => b!.DeclineReason == "INSUFFICIENT_FUNDS"));

        await using var connection = new SqlConnection(db.ConnectionString);
        Assert.Equal(0m, await connection.ExecuteScalarAsync<decimal>(
            "SELECT available FROM accounts WHERE account_id = @accountId", new { accountId }));
    }
}

[Collection("sql")]
public sealed class StandInTests(SqlServerFixture db)
{
    [Fact]
    public async Task Com_a_rede_fora_compra_pequena_e_aprovada_via_stip()
    {
        var (_, token) = await TestData.NewCardAsync(db.ConnectionString, limit: 1000m);
        await using var factory = new AuthorizationApiFactory(new FakeNetworkHandler { Down = true });
        var client = factory.CreateClient();

        var response = await Api.AuthorizeAsync(client, $"stip-{Guid.NewGuid():N}", token, 10m);
        var body = await response.Content.ReadFromJsonAsync<AuthorizationResponse>();

        Assert.Equal("APPROVED", body!.Status);

        await using var connection = new SqlConnection(db.ConnectionString);
        Assert.True(await connection.ExecuteScalarAsync<bool>(
            "SELECT stand_in FROM authorizations WHERE authorization_id = @id", new { id = body.AuthorizationId }));
    }

    [Fact]
    public async Task Com_a_rede_fora_compra_grande_e_recusada()
    {
        var (accountId, token) = await TestData.NewCardAsync(db.ConnectionString, limit: 1000m);
        await using var factory = new AuthorizationApiFactory(new FakeNetworkHandler { Down = true });
        var client = factory.CreateClient();

        var response = await Api.AuthorizeAsync(client, $"stip-{Guid.NewGuid():N}", token, 100m);
        var body = await response.Content.ReadFromJsonAsync<AuthorizationResponse>();

        Assert.Equal("DECLINED", body!.Status);
        Assert.Equal("NETWORK_UNAVAILABLE", body.DeclineReason);

        await using var connection = new SqlConnection(db.ConnectionString);
        Assert.Equal(1000m, await connection.ExecuteScalarAsync<decimal>(
            "SELECT available FROM accounts WHERE account_id = @accountId", new { accountId }));
    }
}