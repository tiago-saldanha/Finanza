using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Finanza.API.Tests.Fixture;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.API.Tests.Endpoints;

public class TransactionCancelEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TenantDbContext _context;

    public TransactionCancelEndpointTests(CustomWebApplicationFactory factory)
    {
        _client  = factory.CreateAuthenticatedClient();
        _scope   = factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    }

    public async Task InitializeAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Transactions");
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task PUT_Cancel_WhenTransactionIsPaid_ShouldReturnBadRequest()
    {
        // Arrange
        var category = Category.Create($"Test Cat {Guid.NewGuid()}", null);
        var account = Account.Create($"Test Acc {Guid.NewGuid()}", AccountType.Checking, 1000);
        
        _context.Categories.Add(category);
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var transaction = Transaction.Create("Paid Transaction", 100.0M, DateTime.Now.AddDays(1), TransactionType.Expense, category.Id, DateTime.Now, accountId: account.Id);
        transaction.Pay(DateTime.Now);
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.PutAsync($"/api/transactions/cancel/{transaction.Id}", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Não é possível cancelar uma transação que já foi paga", body);
    }

    [Fact]
    public async Task PUT_Cancel_Idempotency_ShouldConsistentlyReturnBadRequest()
    {
        // Arrange
        var category = Category.Create($"Test Cat {Guid.NewGuid()}", null);
        var account = Account.Create($"Test Acc {Guid.NewGuid()}", AccountType.Checking, 1000);
        
        _context.Categories.Add(category);
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var transaction = Transaction.Create("Paid Transaction", 100.0M, DateTime.Now.AddDays(1), TransactionType.Expense, category.Id, DateTime.Now, accountId: account.Id);
        transaction.Pay(DateTime.Now);
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var response1 = await _client.PutAsync($"/api/transactions/cancel/{transaction.Id}", null);
        var response2 = await _client.PutAsync($"/api/transactions/cancel/{transaction.Id}", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }
}
