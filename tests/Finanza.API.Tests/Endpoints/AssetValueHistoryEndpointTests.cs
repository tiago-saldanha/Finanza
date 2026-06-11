using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Finanza.API.Tests.Fixture;
using Finanza.Application.DTOs.Responses;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.API.Tests.Endpoints;

public class AssetValueHistoryEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient      _client;
    private readonly IServiceScope   _scope;
    private readonly TenantDbContext _context;

    public AssetValueHistoryEndpointTests(CustomWebApplicationFactory factory)
    {
        _client  = factory.CreateAuthenticatedClient();
        _scope   = factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    }

    public async Task InitializeAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Investments");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM AssetValueHistories");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Transactions");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Categories");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Accounts");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM PatrimonySnapshots");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Assets");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Liabilities");
    }

    public Task DisposeAsync() { _scope.Dispose(); return Task.CompletedTask; }

    private static StringContent Json(object obj)
        => new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    [Fact]
    public async Task POST_AssetValue_ShouldUpdateAssetAndReturnHistory()
    {
        var asset   = await (await _client.PostAsync("/api/assets/", Json(new { name = "ImÃ³vel", type = 2, value = 300_000 })))
                        .Content.ReadFromJsonAsync<AssetResponse>();

        var req      = new { value = 320_000, date = DateTime.Today };
        var response = await _client.PostAsync($"/api/assets/{asset!.Id}/value", Json(req));
        var result   = await response.Content.ReadFromJsonAsync<AssetValueHistoryResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(320_000, result!.Value);
        Assert.Equal(asset.Id, result.AssetId);
    }

    [Fact]
    public async Task GET_AssetValueHistory_ShouldReturnHistory()
    {
        var asset = await (await _client.PostAsync("/api/assets/", Json(new { name = "ImÃ³vel", type = 2, value = 300_000 })))
                        .Content.ReadFromJsonAsync<AssetResponse>();

        await _client.PostAsync($"/api/assets/{asset!.Id}/value", Json(new { value = 310_000, date = new DateTime(2025, 1, 1) }));
        await _client.PostAsync($"/api/assets/{asset.Id}/value",  Json(new { value = 320_000, date = new DateTime(2025, 2, 1) }));

        var response = await _client.GetAsync($"/api/assets/{asset.Id}/value-history");
        var result   = await response.Content.ReadFromJsonAsync<List<AssetValueHistoryResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, result!.Count);
    }

    [Fact]
    public async Task GET_AssetValueHistory_WhenEmpty_ShouldReturnEmptyList()
    {
        var asset = await (await _client.PostAsync("/api/assets/", Json(new { name = "Carro", type = 1, value = 50_000 })))
                        .Content.ReadFromJsonAsync<AssetResponse>();

        var response = await _client.GetAsync($"/api/assets/{asset!.Id}/value-history");
        var result   = await response.Content.ReadFromJsonAsync<List<AssetValueHistoryResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(result!);
    }
}

