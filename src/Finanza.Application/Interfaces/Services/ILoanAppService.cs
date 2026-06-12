using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface ILoanAppService
    {
        Task<IEnumerable<LoanResponse>> GetAllAsync();
        Task<LoanResponse>              GetByIdAsync(Guid id);
        Task<LoanSummaryResponse>       GetSummaryAsync();
        Task<LoanResponse>              CreateAsync(LoanRequest request);
        Task<LoanResponse>              UpdateAsync(Guid id, LoanRequest request);
        Task                            DeleteAsync(Guid id);
        Task<LoanInstallmentResponse>   PayInstallmentAsync(Guid installmentId, PayInstallmentRequest request);
        Task<LoanInstallmentResponse>   UnpayInstallmentAsync(Guid installmentId);
    }
}
