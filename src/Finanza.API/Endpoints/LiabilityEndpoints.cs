using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class LiabilityEndpoints
    {
        public static void MapLiabilityEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/liabilities").RequireAuthorization();

            group.MapGet("/all", async (ILiabilityAppService service) =>
                Results.Ok(await service.GetAllAsync()));

            group.MapGet("/{id:guid}", async (Guid id, ILiabilityAppService service) =>
                Results.Ok(await service.GetByIdAsync(id)));

            group.MapPost("/", async (LiabilityRequest request, ILiabilityAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/liabilities/{result.Id}", result);
            });

            group.MapPut("/{id:guid}", async (Guid id, LiabilityRequest request, ILiabilityAppService service) =>
                Results.Ok(await service.UpdateAsync(id, request)));

            group.MapDelete("/{id:guid}", async (Guid id, ILiabilityAppService service) =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            });
        }
    }
}
