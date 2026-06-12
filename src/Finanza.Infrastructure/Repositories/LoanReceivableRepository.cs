using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class LoanReceivableRepository(TenantDbContext context) : ILoanReceivableRepository
    {
        public async Task<IEnumerable<LoanReceivable>> GetAllAsync()
            => await context.LoanReceivables.AsNoTracking().Include(l => l.Installments).ToListAsync();

        public async Task<LoanReceivable> GetByIdAsync(Guid id)
            => await context.LoanReceivables.Include(l => l.Installments).FirstOrDefaultAsync(l => l.Id == id)
               ?? throw new EntityNotFoundInfraException("Empréstimo não encontrado");

        public async Task AddAsync(LoanReceivable loan) => await context.LoanReceivables.AddAsync(loan);

        public Task UpdateAsync(LoanReceivable loan) { context.LoanReceivables.Update(loan); return Task.CompletedTask; }

        public Task DeleteAsync(LoanReceivable loan) { context.LoanReceivables.Remove(loan); return Task.CompletedTask; }

        public async Task<LoanInstallment> GetInstallmentByIdAsync(Guid installmentId)
            => await context.LoanInstallments.FindAsync(installmentId)
               ?? throw new EntityNotFoundInfraException("Parcela não encontrada");
    }
}
