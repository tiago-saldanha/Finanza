using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class AssetEndpoints
    {
        public static void MapAssetEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/assets").RequireAuthorization();

            group.MapGet("/all", async (IAssetAppService service) =>
                Results.Ok(await service.GetAllAsync()));

            group.MapGet("/{id:guid}", async (Guid id, IAssetAppService service) =>
                Results.Ok(await service.GetByIdAsync(id)));

            group.MapPost("/", async (AssetRequest request, IAssetAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/assets/{result.Id}", result);
            });

            group.MapPut("/{id:guid}", async (Guid id, AssetRequest request, IAssetAppService service) =>
                Results.Ok(await service.UpdateAsync(id, request)));

            group.MapDelete("/{id:guid}", async (Guid id, IAssetAppService service) =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            });
        }
    }
}
