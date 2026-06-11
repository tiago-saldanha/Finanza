using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class InvestmentRepository(TenantDbContext context) : IInvestmentRepository
    {
        public async Task<IEnumerable<Investment>> GetAllAsync()
            => await context.Investments.AsNoTracking().ToListAsync();

        public async Task<Investment> GetByIdAsync(Guid id)
            => await context.Investments.FindAsync(id)
               ?? throw new EntityNotFoundInfraException("Investimento não encontrado");

        public async Task AddAsync(Investment investment)
            => await context.Investments.AddAsync(investment);

        public Task UpdateAsync(Investment investment)
        {
            context.Investments.Update(investment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Investment investment)
        {
            context.Investments.Remove(investment);
            return Task.CompletedTask;
        }
    }
}
