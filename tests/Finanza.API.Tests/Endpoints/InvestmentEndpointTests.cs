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

public class InvestmentEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient      _client;
    private readonly IServiceScope   _scope;
    private readonly TenantDbContext _context;

    public InvestmentEndpointTests(CustomWebApplicationFactory factory)
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
    public async Task POST_Investment_ShouldCreate()
    {
        var req      = new { name = "Tesouro Selic", type = 0, investedAmount = 10_000, currentValue = 10_500 };
        var response = await _client.PostAsync("/api/investments/", Json(req));
        var result   = await response.Content.ReadFromJsonAsync<InvestmentResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Tesouro Selic", result!.Name);
        Assert.Equal("RendaFixa",     result.Type);
        Assert.Equal(500,             result.Return);
        Assert.Equal(5,               result.ReturnRate);
    }

    [Fact]
    public async Task GET_Portfolio_ShouldReturnKPIs()
    {
        await _client.PostAsync("/api/investments/", Json(new { name = "Tesouro", type = 0, investedAmount = 10_000, currentValue = 10_500 }));
        await _client.PostAsync("/api/investments/", Json(new { name = "PETR4",   type = 1, investedAmount = 5_000,  currentValue = 4_500  }));

        var response  = await _client.GetAsync("/api/investments/portfolio");
        var portfolio = await response.Content.ReadFromJsonAsync<InvestmentPortfolioResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(15_000, portfolio!.TotalInvested);
        Assert.Equal(15_000, portfolio.TotalCurrent);
        Assert.Equal(0,      portfolio.TotalReturn);
        Assert.Equal(2,      portfolio.Investments.Count());
        Assert.Equal(2,      portfolio.Allocations.Count());
    }

    [Fact]
    public async Task DELETE_Investment_ShouldRemove()
    {
        var created = await (await _client.PostAsync("/api/investments/",
            Json(new { name = "FII XPLG11", type = 2, investedAmount = 2000, currentValue = 2100 })))
            .Content.ReadFromJsonAsync<InvestmentResponse>();

        var del = await _client.DeleteAsync($"/api/investments/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

        var portfolio = await (await _client.GetAsync("/api/investments/portfolio"))
            .Content.ReadFromJsonAsync<InvestmentPortfolioResponse>();
        Assert.Empty(portfolio!.Investments);
    }
}
