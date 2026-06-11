using Finanza.Domain.Entities;

namespace Finanza.Domain.Repositories
{
    public interface IPatrimonySnapshotRepository
    {
        Task<IEnumerable<PatrimonySnapshot>> GetAllAsync();
        Task AddAsync(PatrimonySnapshot snapshot);
    }
}
