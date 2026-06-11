using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Finanza.API.Tests.Fixture;
using Finanza.Application.DTOs.Responses;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.API.Tests.Endpoints;

public class CategoryEndpointTests
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TenantDbContext _context;

    public CategoryEndpointTests(CustomWebApplicationFactory factory)
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

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task POST_ShouldCreateCategory()
    {
        var request = new { name = "Test", description = "Description" };

        var response = await _client.PostAsync("/api/categories", GetContent(request));
        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(request.name, category!.Name);
    }

    [Fact]
    public async Task GET_WhenIdIsProvided_ShouldReturnCategory()
    {
        var request = new { name = "Test", description = "Description" };

        var createResult = await _client.PostAsync("/api/categories", GetContent(request));
        var created = await createResult.Content.ReadFromJsonAsync<CategoryResponse>();

        var response = await _client.GetAsync($"/api/categories/{created!.Id}");
        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created.Id,          category!.Id);
        Assert.Equal(created.Name,        category.Name);
        Assert.Equal(created.Description, category.Description);
    }

    [Fact]
    public async Task GET_ShouldReturnAllCategories()
    {
        var request = new { name = "Test", description = "Description" };
        var postResponse = await _client.PostAsync("/api/categories", GetContent(request));
        var postBody = await postResponse.Content.ReadAsStringAsync();
        Assert.True(postResponse.IsSuccessStatusCode, $"POST falhou ({postResponse.StatusCode}): {postBody}");

        var response = await _client.GetAsync("/api/categories/all");
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.OK, $"GET /all falhou ({response.StatusCode}): {body}");

        var categories = System.Text.Json.JsonSerializer.Deserialize<List<CategoryResponse>>(body,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(categories);
        Assert.NotEmpty(categories!);
        Assert.Single(categories!);
    }

    [Fact]
    public async Task PUT_WhenIdAndRequestAreValid_ShouldUpdateCategory()
    {
        var createRequest = new { name = "Original", description = "Original Description" };
        var createResult  = await _client.PostAsync("/api/categories", GetContent(createRequest));
        var created       = await createResult.Content.ReadFromJsonAsync<CategoryResponse>();

        var updateRequest = new { name = "Updated", description = "Updated Description" };
        var response      = await _client.PutAsync($"/api/categories/{created!.Id}", GetContent(updateRequest));
        var updated       = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created.Id,                updated!.Id);
        Assert.Equal(updateRequest.name,        updated.Name);
        Assert.Equal(updateRequest.description, updated.Description);
    }

    private static StringContent GetContent(object request)
        => new(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
}
