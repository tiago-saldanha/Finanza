using Finanza.Domain.Entities;

namespace Finanza.Domain.Repositories
{
    public interface ILoanPayableRepository
    {
        Task<IEnumerable<LoanPayable>> GetAllAsync();
        Task<LoanPayable> GetByIdAsync(Guid id);
        Task AddAsync(LoanPayable loan);
        Task UpdateAsync(LoanPayable loan);
        Task DeleteAsync(LoanPayable loan);
        Task<LoanPayableInstallment> GetInstallmentByIdAsync(Guid installmentId);
    }
}
