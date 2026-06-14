using Finanza.Application.DTOs.Requests;
using Finanza.Application.Interfaces.Services;

namespace Finanza.API.Endpoints
{
    public static class LoanPayableEndpoints
    {
        public static void MapLoanPayableEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/loan-payables").RequireAuthorization();

            group.MapGet("/", async (ILoanPayableAppService service) =>
                Results.Ok(await service.GetAllAsync()));

            group.MapGet("/summary", async (ILoanPayableAppService service) =>
                Results.Ok(await service.GetSummaryAsync()));

            group.MapGet("/{id:guid}", async (Guid id, ILoanPayableAppService service) =>
                Results.Ok(await service.GetByIdAsync(id)));

            group.MapPost("/", async (LoanPayableRequest request, ILoanPayableAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/loan-payables/{result.Id}", result);
            });

            group.MapPut("/{id:guid}", async (Guid id, LoanPayableRequest request, ILoanPayableAppService service) =>
                Results.Ok(await service.UpdateAsync(id, request)));

            group.MapDelete("/{id:guid}", async (Guid id, ILoanPayableAppService service) =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            });

            group.MapPost("/installments/{installmentId:guid}/pay", async (Guid installmentId, PayPayableInstallmentRequest request, ILoanPayableAppService service) =>
                Results.Ok(await service.PayInstallmentAsync(installmentId, request)));

            group.MapPost("/installments/{installmentId:guid}/unpay", async (Guid installmentId, ILoanPayableAppService service) =>
                Results.Ok(await service.UnpayInstallmentAsync(installmentId)));
        }
    }
}
