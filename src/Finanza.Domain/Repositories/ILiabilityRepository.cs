using Finanza.Domain.Entities;

namespace Finanza.Domain.Repositories
{
    public interface ILiabilityRepository
    {
        Task<IEnumerable<Liability>> GetAllAsync();
        Task<Liability> GetByIdAsync(Guid id);
        Task AddAsync(Liability liability);
        Task UpdateAsync(Liability liability);
        Task DeleteAsync(Liability liability);
    }
}
