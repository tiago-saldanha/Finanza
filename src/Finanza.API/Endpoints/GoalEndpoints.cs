using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class GoalEndpoints
    {
        public static void MapGoalEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/goals").RequireAuthorization();

            group.MapGet("/", async (IGoalAppService service) =>
                Results.Ok(await service.GetAllAsync()));

            group.MapGet("/{id:guid}", async (Guid id, IGoalAppService service) =>
                Results.Ok(await service.GetByIdAsync(id)));

            group.MapPost("/", async (GoalRequest request, IGoalAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/goals/{result.Id}", result);
            });

            group.MapPut("/{id:guid}", async (Guid id, GoalRequest request, IGoalAppService service) =>
                Results.Ok(await service.UpdateAsync(id, request)));

            group.MapPost("/{id:guid}/contribute", async (Guid id, ContributeGoalRequest request, IGoalAppService service) =>
                Results.Ok(await service.ContributeAsync(id, request)));

            group.MapDelete("/{id:guid}", async (Guid id, IGoalAppService service) =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            });
        }
    }
}
