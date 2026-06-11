using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class FinancialAccountEndpoints
    {
        public static void MapFinancialAccountEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/financial-accounts")
                .WithTags("FinancialAccounts")
                .RequireAuthorization();

            group.MapGet("/all", async (IFinancialAccountAppService service)
                => Results.Ok(await service.GetAllAsync()));

            group.MapGet("/{id:guid}", async (Guid id, IFinancialAccountAppService service)
                => Results.Ok(await service.GetByIdAsync(id)));

            group.MapPost("/", async (FinancialAccountRequest request, IFinancialAccountAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/financial-accounts/{result.Id}", result);
            });

            group.MapPut("/{id:guid}", async (Guid id, FinancialAccountRequest request, IFinancialAccountAppService service)
                => Results.Ok(await service.UpdateAsync(id, request)));

            group.MapDelete("/{id:guid}", async (Guid id, IFinancialAccountAppService service) =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            });
        }
    }
}
