using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IFinancialPlanningAppService
    {
        Task<FinancialPlanningResponse> GetAsync(int year, int month);
    }
}
