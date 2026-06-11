using Finanza.Domain.Entities;

namespace Finanza.Domain.Repositories
{
    public interface IGoalRepository
    {
        Task<IEnumerable<Goal>> GetAllAsync();
        Task<Goal> GetByIdAsync(Guid id);
        Task AddAsync(Goal goal);
        Task UpdateAsync(Goal goal);
        Task DeleteAsync(Goal goal);
    }
}
