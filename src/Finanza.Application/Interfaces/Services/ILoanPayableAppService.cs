using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface ILoanPayableAppService
    {
        Task<IEnumerable<LoanPayableResponse>>     GetAllAsync();
        Task<LoanPayableResponse>                  GetByIdAsync(Guid id);
        Task<LoanPayableSummaryResponse>           GetSummaryAsync();
        Task<LoanPayableResponse>                  CreateAsync(LoanPayableRequest request);
        Task<LoanPayableResponse>                  UpdateAsync(Guid id, LoanPayableRequest request);
        Task                                       DeleteAsync(Guid id);
        Task<LoanPayableInstallmentResponse>       PayInstallmentAsync(Guid installmentId, PayPayableInstallmentRequest request);
        Task<LoanPayableInstallmentResponse>       UnpayInstallmentAsync(Guid installmentId);
    }
}
