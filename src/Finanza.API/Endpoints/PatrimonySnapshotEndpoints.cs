using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class PatrimonySnapshotEndpoints
    {
        public static void MapPatrimonySnapshotEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/patrimony-snapshots").RequireAuthorization();

            group.MapGet("/", async (IPatrimonySnapshotAppService service) =>
                Results.Ok(await service.GetAllAsync()));

            group.MapPost("/", async (IPatrimonySnapshotAppService service) =>
            {
                var result = await service.CreateAsync();
                return Results.Created($"/api/patrimony-snapshots/{result.Id}", result);
            });
        }
    }
}
