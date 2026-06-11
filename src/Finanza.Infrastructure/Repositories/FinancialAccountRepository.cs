using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class FinancialAccountRepository(TenantDbContext context) : IFinancialAccountRepository
    {
        public async Task<IEnumerable<Account>> GetAllAsync()
            => await context.Accounts.AsNoTracking().Include(a => a.Transactions).ToListAsync();

        public async Task<Account> GetByIdAsync(Guid id)
            => await context.Accounts.AsNoTracking().Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == id)
               ?? throw new EntityNotFoundInfraException("Conta não encontrada");

        public async Task AddAsync(Account account)
            => await context.Accounts.AddAsync(account);

        public Task UpdateAsync(Account account)
        {
            context.Accounts.Update(account);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Account account)
        {
            context.Accounts.Remove(account);
            return Task.CompletedTask;
        }
    }
}
