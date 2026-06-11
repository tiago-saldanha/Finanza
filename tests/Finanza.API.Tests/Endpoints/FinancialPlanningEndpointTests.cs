using System.Net;
using System.Net.Http.Json;
using Finanza.API.Tests.Fixture;
using Finanza.Application.DTOs.Responses;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.API.Tests.Endpoints;

public class FinancialPlanningEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TenantDbContext _context;

    public FinancialPlanningEndpointTests(CustomWebApplicationFactory factory)
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
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Investments");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM AssetValueHistories");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM PatrimonySnapshots");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Assets");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Liabilities");
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GET_ShouldReturnPlanningKPIs()
    {
        var now = DateTime.UtcNow;

        var response = await _client.GetAsync($"/api/planning?year={now.Year}&month={now.Month}");
        var result   = await response.Content.ReadFromJsonAsync<FinancialPlanningResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(now.Year,  result!.Year);
        Assert.Equal(now.Month, result.Month);
        Assert.Equal(6, result.EmergencyFundTarget);
        Assert.Equal(20, result.Rule20Target);
    }

    [Fact]
    public async Task GET_WithNoTransactions_ShouldReturnZeroKPIs()
    {
        var response = await _client.GetAsync("/api/planning?year=2024&month=1");
        var result   = await response.Content.ReadFromJsonAsync<FinancialPlanningResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, result!.MonthlyRevenue);
        Assert.Equal(0, result.SavingsRate);
        Assert.Equal(0, result.EmergencyFundMonths);
    }

    [Fact]
    public async Task GET_WithoutParams_ShouldUseCurrentMonthByDefault()
    {
        var now      = DateTime.UtcNow;
        var response = await _client.GetAsync("/api/planning");
        var result   = await response.Content.ReadFromJsonAsync<FinancialPlanningResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(now.Year,  result!.Year);
        Assert.Equal(now.Month, result.Month);
    }
}

