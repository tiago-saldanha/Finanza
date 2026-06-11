using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class InvestmentEndpoints
    {
        public static void MapInvestmentEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/investments").RequireAuthorization();

            group.MapGet("/portfolio", async (IInvestmentAppService service) =>
                Results.Ok(await service.GetPortfolioAsync()));

            group.MapGet("/{id:guid}", async (Guid id, IInvestmentAppService service) =>
                Results.Ok(await service.GetByIdAsync(id)));

            group.MapPost("/", async (InvestmentRequest request, IInvestmentAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/investments/{result.Id}", result);
            });

            group.MapPut("/{id:guid}", async (Guid id, InvestmentRequest request, IInvestmentAppService service) =>
                Results.Ok(await service.UpdateAsync(id, request)));

            group.MapDelete("/{id:guid}", async (Guid id, IInvestmentAppService service) =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            });
        }
    }
}
