using Finanza.Domain.Entities;

namespace Finanza.Domain.Repositories
{
    public interface ILoanReceivableRepository
    {
        Task<IEnumerable<LoanReceivable>> GetAllAsync();
        Task<LoanReceivable> GetByIdAsync(Guid id);
        Task AddAsync(LoanReceivable loan);
        Task UpdateAsync(LoanReceivable loan);
        Task DeleteAsync(LoanReceivable loan);
        Task<LoanInstallment> GetInstallmentByIdAsync(Guid installmentId);
    }
}
