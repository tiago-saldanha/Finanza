using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IFinancialAccountAppService
    {
        Task<IEnumerable<FinancialAccountResponse>> GetAllAsync();
        Task<FinancialAccountResponse> GetByIdAsync(Guid id);
        Task<FinancialAccountResponse> CreateAsync(FinancialAccountRequest request);
        Task<FinancialAccountResponse> UpdateAsync(Guid id, FinancialAccountRequest request);
        Task DeleteAsync(Guid id);
    }
}
