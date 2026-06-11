using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class AssetValueHistoryRepository(TenantDbContext context) : IAssetValueHistoryRepository
    {
        public async Task<IEnumerable<AssetValueHistory>> GetByAssetIdAsync(Guid assetId)
            => await context.AssetValueHistories
                .AsNoTracking()
                .Where(h => h.AssetId == assetId)
                .ToListAsync();

        public async Task AddAsync(AssetValueHistory history)
            => await context.AssetValueHistories.AddAsync(history);
    }
}
