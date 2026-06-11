using Finanza.Domain.Entities;

namespace Finanza.Domain.Repositories
{
    public interface IInvestmentRepository
    {
        Task<IEnumerable<Investment>> GetAllAsync();
        Task<Investment> GetByIdAsync(Guid id);
        Task AddAsync(Investment investment);
        Task UpdateAsync(Investment investment);
        Task DeleteAsync(Investment investment);
    }
}
