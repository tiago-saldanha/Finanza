using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class LoanEndpoints
    {
        public static void MapLoanEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/loans").RequireAuthorization();

            group.MapGet("/", async (ILoanAppService service) =>
                Results.Ok(await service.GetAllAsync()));

            group.MapGet("/summary", async (ILoanAppService service) =>
                Results.Ok(await service.GetSummaryAsync()));

            group.MapGet("/{id:guid}", async (Guid id, ILoanAppService service) =>
                Results.Ok(await service.GetByIdAsync(id)));

            group.MapPost("/", async (LoanRequest request, ILoanAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/loans/{result.Id}", result);
            });

            group.MapPut("/{id:guid}", async (Guid id, LoanRequest request, ILoanAppService service) =>
                Results.Ok(await service.UpdateAsync(id, request)));

            group.MapDelete("/{id:guid}", async (Guid id, ILoanAppService service) =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            });

            group.MapPost("/installments/{installmentId:guid}/pay", async (Guid installmentId, PayInstallmentRequest request, ILoanAppService service) =>
                Results.Ok(await service.PayInstallmentAsync(installmentId, request)));

            group.MapPost("/installments/{installmentId:guid}/unpay", async (Guid installmentId, ILoanAppService service) =>
                Results.Ok(await service.UnpayInstallmentAsync(installmentId)));
        }
    }
}
