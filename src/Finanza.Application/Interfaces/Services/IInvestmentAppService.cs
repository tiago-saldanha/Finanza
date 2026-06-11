using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IInvestmentAppService
    {
        Task<InvestmentPortfolioResponse> GetPortfolioAsync();
        Task<InvestmentResponse> GetByIdAsync(Guid id);
        Task<InvestmentResponse> CreateAsync(InvestmentRequest request);
        Task<InvestmentResponse> UpdateAsync(Guid id, InvestmentRequest request);
        Task DeleteAsync(Guid id);
    }
}
