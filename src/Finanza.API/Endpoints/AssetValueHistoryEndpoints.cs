using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class AssetValueHistoryEndpoints
    {
        public static void MapAssetValueHistoryEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/assets").RequireAuthorization();

            group.MapGet("/{id:guid}/value-history", async (Guid id, IAssetValueHistoryAppService service) =>
                Results.Ok(await service.GetByAssetIdAsync(id)));

            group.MapPost("/{id:guid}/value", async (Guid id, UpdateAssetValueRequest request, IAssetValueHistoryAppService service) =>
            {
                var result = await service.UpdateValueAsync(id, request);
                return Results.Created($"/api/assets/{id}/value-history", result);
            });
        }
    }
}
