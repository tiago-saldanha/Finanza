using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class NetWorthEndpoints
    {
        public static void MapNetWorthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/net-worth").RequireAuthorization();

            group.MapGet("/", async (INetWorthAppService service) =>
                Results.Ok(await service.GetAsync()));
        }
    }
}
