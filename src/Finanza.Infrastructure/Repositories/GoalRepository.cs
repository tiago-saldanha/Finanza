using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class GoalRepository(TenantDbContext context) : IGoalRepository
    {
        public async Task<IEnumerable<Goal>> GetAllAsync()
            => await context.Goals.AsNoTracking().ToListAsync();

        public async Task<Goal> GetByIdAsync(Guid id)
            => await context.Goals.FindAsync(id)
               ?? throw new EntityNotFoundInfraException("Meta não encontrada");

        public async Task AddAsync(Goal goal) => await context.Goals.AddAsync(goal);

        public Task UpdateAsync(Goal goal) { context.Goals.Update(goal); return Task.CompletedTask; }

        public Task DeleteAsync(Goal goal) { context.Goals.Remove(goal); return Task.CompletedTask; }
    }
}
