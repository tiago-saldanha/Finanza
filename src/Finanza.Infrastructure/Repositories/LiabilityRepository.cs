using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class LiabilityRepository(TenantDbContext context) : ILiabilityRepository
    {
        public async Task<IEnumerable<Liability>> GetAllAsync()
            => await context.Liabilities.AsNoTracking().Include(l => l.Installments).ToListAsync();

        public async Task<Liability> GetByIdAsync(Guid id)
            => await context.Liabilities.Include(l => l.Installments).FirstOrDefaultAsync(l => l.Id == id)
               ?? throw new EntityNotFoundInfraException("Passivo não encontrado");

        public async Task<LiabilityInstallment> GetInstallmentByIdAsync(Guid installmentId)
            => await context.LiabilityInstallments.FirstOrDefaultAsync(i => i.Id == installmentId)
               ?? throw new EntityNotFoundInfraException("Parcela não encontrada");

        public async Task AddAsync(Liability liability)
            => await context.Liabilities.AddAsync(liability);

        public Task UpdateAsync(Liability liability)
        {
            context.Liabilities.Update(liability);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Liability liability)
        {
            context.Liabilities.Remove(liability);
            return Task.CompletedTask;
        }
    }
}
