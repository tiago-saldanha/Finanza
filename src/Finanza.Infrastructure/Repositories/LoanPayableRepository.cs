using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class LoanPayableRepository(TenantDbContext context) : ILoanPayableRepository
    {
        public async Task<IEnumerable<LoanPayable>> GetAllAsync()
            => await context.LoanPayables.AsNoTracking().Include(l => l.Installments).ToListAsync();

        public async Task<LoanPayable> GetByIdAsync(Guid id)
            => await context.LoanPayables.Include(l => l.Installments).FirstOrDefaultAsync(l => l.Id == id)
               ?? throw new EntityNotFoundInfraException("Empréstimo obtido não encontrado");

        public async Task AddAsync(LoanPayable loan) => await context.LoanPayables.AddAsync(loan);

        public Task UpdateAsync(LoanPayable loan) { context.LoanPayables.Update(loan); return Task.CompletedTask; }

        public Task DeleteAsync(LoanPayable loan) { context.LoanPayables.Remove(loan); return Task.CompletedTask; }

        public async Task<LoanPayableInstallment> GetInstallmentByIdAsync(Guid installmentId)
            => await context.LoanPayableInstallments.FindAsync(installmentId)
               ?? throw new EntityNotFoundInfraException("Parcela não encontrada");
    }
}
