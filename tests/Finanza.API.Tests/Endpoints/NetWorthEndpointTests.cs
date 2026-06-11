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

public class NetWorthEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient    _client;
    private readonly IServiceScope _scope;
    private readonly TenantDbContext _context;

    public NetWorthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client  = factory.CreateAuthenticatedClient();
        _scope   = factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    }

    public async Task InitializeAsync()
    {
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
    public async Task GET_NetWorth_WithNoData_ShouldReturnZero()
    {
        var response = await _client.GetAsync("/api/net-worth/");
        var result   = await response.Content.ReadFromJsonAsync<NetWorthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, result!.NetWorth);
        Assert.Equal(0, result.TotalAssets);
        Assert.Equal(0, result.TotalLiabilities);
    }

    [Fact]
    public async Task POST_Asset_ShouldCreate()
    {
        var req      = new { name = "Imóvel", type = 2, value = 300_000 }; // 2 = Property
        var response = await _client.PostAsync("/api/assets/", Json(req));
        var result   = await response.Content.ReadFromJsonAsync<AssetResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Imóvel",   result!.Name);
        Assert.Equal("Property", result.Type);
    }

    [Fact]
    public async Task POST_Liability_ShouldCreate()
    {
        var req      = new { name = "Financiamento", type = 0, value = 150_000 }; // 0 = Financing
        var response = await _client.PostAsync("/api/liabilities/", Json(req));
        var result   = await response.Content.ReadFromJsonAsync<LiabilityResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Financiamento", result!.Name);
        Assert.Equal("Financing",      result.Type);
    }

    [Fact]
    public async Task GET_NetWorth_AfterInserts_ShouldComputeCorrectly()
    {
        await _client.PostAsync("/api/assets/",      Json(new { name = "Imóvel", type = 2, value = 300_000 }));
        await _client.PostAsync("/api/liabilities/", Json(new { name = "Financiamento", type = 0, value = 150_000 }));

        var response = await _client.GetAsync("/api/net-worth/");
        var result   = await response.Content.ReadFromJsonAsync<NetWorthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(300_000, result!.TotalAssets);
        Assert.Equal(150_000, result.TotalLiabilities);
        Assert.Equal(150_000, result.NetWorth);
    }

    [Fact]
    public async Task DELETE_Asset_ShouldRemove()
    {
        var created = await (await _client.PostAsync("/api/assets/",
            Json(new { name = "Carro", type = 1, value = 50_000 })))
            .Content.ReadFromJsonAsync<AssetResponse>();

        var del = await _client.DeleteAsync($"/api/assets/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
    }

    [Fact]
    public async Task DELETE_Liability_ShouldRemove()
    {
        var created = await (await _client.PostAsync("/api/liabilities/",
            Json(new { name = "Empréstimo", type = 1, value = 10_000 })))
            .Content.ReadFromJsonAsync<LiabilityResponse>();

        var del = await _client.DeleteAsync($"/api/liabilities/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
    }
}
