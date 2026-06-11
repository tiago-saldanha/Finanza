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

public class FinancialAccountEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TenantDbContext _context;

    public FinancialAccountEndpointTests(CustomWebApplicationFactory factory)
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
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task POST_ShouldCreateAccount()
    {
        var request = new { name = "Conta Corrente", type = 0, initialBalance = 1000 }; // 0 = Checking

        var response = await _client.PostAsync("/api/financial-accounts", GetContent(request));
        var account  = await response.Content.ReadFromJsonAsync<FinancialAccountResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(request.name, account!.Name);
        Assert.Equal("Checking", account.Type);
        Assert.Equal(1000, account.InitialBalance);
    }

    [Fact]
    public async Task GET_WhenIdIsProvided_ShouldReturnAccount()
    {
        var create  = new { name = "Poupança", type = 1, initialBalance = 500 }; // 1 = Savings
        var created = await (await _client.PostAsync("/api/financial-accounts", GetContent(create)))
            .Content.ReadFromJsonAsync<FinancialAccountResponse>();

        var response = await _client.GetAsync($"/api/financial-accounts/{created!.Id}");
        var account  = await response.Content.ReadFromJsonAsync<FinancialAccountResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created.Id,   account!.Id);
        Assert.Equal(created.Name, account.Name);
    }

    [Fact]
    public async Task GET_ShouldReturnAllAccounts()
    {
        var postResponse = await _client.PostAsync("/api/financial-accounts",
            GetContent(new { name = "Carteira", type = 2, initialBalance = 0 })); // 2 = Wallet
        Assert.True(postResponse.IsSuccessStatusCode,
            $"POST falhou ({postResponse.StatusCode}): {await postResponse.Content.ReadAsStringAsync()}");

        var response = await _client.GetAsync("/api/financial-accounts/all");
        var body     = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var accounts = JsonSerializer.Deserialize<List<FinancialAccountResponse>>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(accounts);
        Assert.Single(accounts!);
    }

    [Fact]
    public async Task PUT_ShouldUpdateAccount()
    {
        var created = await (await _client.PostAsync("/api/financial-accounts",
            GetContent(new { name = "Original", type = 0, initialBalance = 100 }))) // 0 = Checking
            .Content.ReadFromJsonAsync<FinancialAccountResponse>();

        var updateRequest = new { name = "Atualizada", type = 1, initialBalance = 200 }; // 1 = Savings
        var response = await _client.PutAsync($"/api/financial-accounts/{created!.Id}", GetContent(updateRequest));
        var updated  = await response.Content.ReadFromJsonAsync<FinancialAccountResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Atualizada", updated!.Name);
        Assert.Equal("Savings",    updated.Type);
        Assert.Equal(200,          updated.InitialBalance);
    }

    [Fact]
    public async Task DELETE_ShouldRemoveAccount()
    {
        var created = await (await _client.PostAsync("/api/financial-accounts",
            GetContent(new { name = "Para Deletar", type = 2, initialBalance = 0 }))) // 2 = Wallet
            .Content.ReadFromJsonAsync<FinancialAccountResponse>();

        var response = await _client.DeleteAsync($"/api/financial-accounts/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/financial-accounts/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static StringContent GetContent(object request)
        => new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
}
