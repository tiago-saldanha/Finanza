using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Exceptions;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class GoalAppService(IGoalRepository repository, IUnitOfWork unitOfWork) : IGoalAppService
    {
        public async Task<IEnumerable<GoalResponse>> GetAllAsync()
            => (await repository.GetAllAsync()).Select(GoalResponse.Create);

        public async Task<GoalResponse> GetByIdAsync(Guid id)
            => GoalResponse.Create(await repository.GetByIdAsync(id));

        public async Task<GoalResponse> CreateAsync(GoalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new GoalNameAppException();
            var goal = Goal.Create(request.Name, request.TargetAmount, request.CurrentAmount, request.TargetDate);
            await repository.AddAsync(goal);
            await unitOfWork.CommitAsync();
            return GoalResponse.Create(goal);
        }

        public async Task<GoalResponse> UpdateAsync(Guid id, GoalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new GoalNameAppException();
            var goal = await repository.GetByIdAsync(id);
            goal.Update(request.Name, request.TargetAmount, request.CurrentAmount, request.TargetDate);
            await repository.UpdateAsync(goal);
            await unitOfWork.CommitAsync();
            return GoalResponse.Create(goal);
        }

        public async Task<GoalResponse> ContributeAsync(Guid id, ContributeGoalRequest request)
        {
            var goal = await repository.GetByIdAsync(id);
            goal.Contribute(request.Amount);
            await repository.UpdateAsync(goal);
            await unitOfWork.CommitAsync();
            return GoalResponse.Create(goal);
        }

        public async Task DeleteAsync(Guid id)
        {
            var goal = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(goal);
            await unitOfWork.CommitAsync();
        }
    }
}
