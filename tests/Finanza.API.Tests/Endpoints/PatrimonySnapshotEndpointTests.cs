using System.Net;
using System.Net.Http.Json;
using Finanza.API.Tests.Fixture;
using Finanza.Application.DTOs.Responses;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.API.Tests.Endpoints;

public class PatrimonySnapshotEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient      _client;
    private readonly IServiceScope   _scope;
    private readonly TenantDbContext _context;

    public PatrimonySnapshotEndpointTests(CustomWebApplicationFactory factory)
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
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Goals");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Investments");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM AssetValueHistories");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM PatrimonySnapshots");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Assets");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Liabilities");
    }

    public Task DisposeAsync() { _scope.Dispose(); return Task.CompletedTask; }

    [Fact]
    public async Task GET_Snapshots_WhenEmpty_ShouldReturnEmptyList()
    {
        var response = await _client.GetAsync("/api/patrimony-snapshots/");
        var result   = await response.Content.ReadFromJsonAsync<List<PatrimonySnapshotResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(result!);
    }

    [Fact]
    public async Task POST_Snapshot_WithNoAssetsOrLiabilities_ShouldReturnZeros()
    {
        var response = await _client.PostAsync("/api/patrimony-snapshots/", null);
        var result   = await response.Content.ReadFromJsonAsync<PatrimonySnapshotResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(0, result!.TotalAssets);
        Assert.Equal(0, result.TotalLiabilities);
        Assert.Equal(0, result.NetWorth);
    }

    [Fact]
    public async Task POST_Snapshot_ShouldCaptureCurrentNetWorth()
    {
        using var content = System.Net.Http.Json.JsonContent.Create(new { name = "ImÃ³vel", type = 2, value = 300_000 });
        await _client.PostAsync("/api/assets/", content);
        using var content2 = System.Net.Http.Json.JsonContent.Create(new { name = "Financiamento", type = 0, value = 100_000 });
        await _client.PostAsync("/api/liabilities/", content2);

        var response = await _client.PostAsync("/api/patrimony-snapshots/", null);
        var result   = await response.Content.ReadFromJsonAsync<PatrimonySnapshotResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(300_000, result!.TotalAssets);
        Assert.Equal(100_000, result.TotalLiabilities);
        Assert.Equal(200_000, result.NetWorth);
    }

    [Fact]
    public async Task GET_Snapshots_ShouldReturnAllSnapshots()
    {
        await _client.PostAsync("/api/patrimony-snapshots/", null);
        await _client.PostAsync("/api/patrimony-snapshots/", null);

        var response = await _client.GetAsync("/api/patrimony-snapshots/");
        var result   = await response.Content.ReadFromJsonAsync<List<PatrimonySnapshotResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, result!.Count);
    }
}


