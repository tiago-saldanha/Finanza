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

public class GoalEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient      _client;
    private readonly IServiceScope   _scope;
    private readonly TenantDbContext _context;

    public GoalEndpointTests(CustomWebApplicationFactory factory)
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

    private static StringContent Json(object obj)
        => new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    [Fact]
    public async Task POST_Goal_ShouldCreate()
    {
        var req      = new { name = "Viagem", targetAmount = 10_000, currentAmount = 2_500, targetDate = "2026-12-31" };
        var response = await _client.PostAsync("/api/goals/", Json(req));
        var result   = await response.Content.ReadFromJsonAsync<GoalResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Viagem", result!.Name);
        Assert.Equal(25,       result.ProgressRate);
        Assert.Equal(7_500,    result.Remaining);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task POST_Contribute_ShouldIncrementCurrentAmount()
    {
        var created = await (await _client.PostAsync("/api/goals/",
            Json(new { name = "Reserva", targetAmount = 5_000, currentAmount = 1_000, targetDate = "2027-01-01" })))
            .Content.ReadFromJsonAsync<GoalResponse>();

        var response = await _client.PostAsync($"/api/goals/{created!.Id}/contribute", Json(new { amount = 500 }));
        var result   = await response.Content.ReadFromJsonAsync<GoalResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1_500, result!.CurrentAmount);
    }

    [Fact]
    public async Task GET_Goals_ShouldReturnAll()
    {
        await _client.PostAsync("/api/goals/", Json(new { name = "Meta 1", targetAmount = 1000, currentAmount = 0, targetDate = "2027-01-01" }));
        await _client.PostAsync("/api/goals/", Json(new { name = "Meta 2", targetAmount = 2000, currentAmount = 500, targetDate = "2027-06-01" }));

        var response = await _client.GetAsync("/api/goals/");
        var result   = await response.Content.ReadFromJsonAsync<List<GoalResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, result!.Count);
    }

    [Fact]
    public async Task DELETE_Goal_ShouldRemove()
    {
        var created = await (await _client.PostAsync("/api/goals/",
            Json(new { name = "Para Deletar", targetAmount = 500, currentAmount = 0, targetDate = "2026-01-01" })))
            .Content.ReadFromJsonAsync<GoalResponse>();

        var del = await _client.DeleteAsync($"/api/goals/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
    }
}
