using System.Net;
using System.Net.Http.Json;
using Finanza.API.Tests.Fixture;
using Finanza.Application.DTOs.Responses;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.API.Tests.Endpoints;

public class FireEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient      _client;
    private readonly IServiceScope   _scope;
    private readonly TenantDbContext _context;

    public FireEndpointTests(CustomWebApplicationFactory factory)
    {
        _client  = factory.CreateAuthenticatedClient();
        _scope   = factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    }

    public async Task InitializeAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Goals");
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

    [Fact]
    public async Task GET_Fire_WithNoData_ShouldReturnZeros()
    {
        var response = await _client.GetAsync("/api/fire/");
        var result   = await response.Content.ReadFromJsonAsync<FireResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0,  result!.AnnualExpenses);
        Assert.Equal(0,  result.FireGoal);
        Assert.Equal(0,  result.TotalInvested);
        Assert.Equal(-1, result.EstimatedYearsToFire);
    }

    [Fact]
    public async Task GET_Fire_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/fire/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
