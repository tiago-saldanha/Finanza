using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IGoalAppService
    {
        Task<IEnumerable<GoalResponse>> GetAllAsync();
        Task<GoalResponse> GetByIdAsync(Guid id);
        Task<GoalResponse> CreateAsync(GoalRequest request);
        Task<GoalResponse> UpdateAsync(Guid id, GoalRequest request);
        Task<GoalResponse> ContributeAsync(Guid id, ContributeGoalRequest request);
        Task DeleteAsync(Guid id);
    }
}
