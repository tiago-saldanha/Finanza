using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class FinancialPlanningEndpoints
    {
        public static void MapFinancialPlanningEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/planning").RequireAuthorization();

            group.MapGet("/", async (
                IFinancialPlanningAppService service,
                int? year,
                int? month) =>
            {
                var now = DateTime.UtcNow;
                var y = year  ?? now.Year;
                var m = month ?? now.Month;
                var result = await service.GetAsync(y, m);
                return Results.Ok(result);
            });
        }
    }
}
