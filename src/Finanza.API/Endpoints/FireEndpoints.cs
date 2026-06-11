using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class FireEndpoints
    {
        public static void MapFireEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/fire").RequireAuthorization();

            group.MapGet("/", async (IFireAppService service) =>
                Results.Ok(await service.GetAsync()));
        }
    }
}
