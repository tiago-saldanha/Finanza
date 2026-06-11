using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class PatrimonySnapshotRepository(TenantDbContext context) : IPatrimonySnapshotRepository
    {
        public async Task<IEnumerable<PatrimonySnapshot>> GetAllAsync()
            => await context.PatrimonySnapshots.AsNoTracking().ToListAsync();

        public async Task AddAsync(PatrimonySnapshot snapshot)
            => await context.PatrimonySnapshots.AddAsync(snapshot);
    }
}
