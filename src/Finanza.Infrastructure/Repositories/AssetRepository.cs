using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class AssetRepository(TenantDbContext context) : IAssetRepository
    {
        public async Task<IEnumerable<Asset>> GetAllAsync()
            => await context.Assets.AsNoTracking().ToListAsync();

        public async Task<Asset> GetByIdAsync(Guid id)
            => await context.Assets.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id)
               ?? throw new EntityNotFoundInfraException("Ativo não encontrado");

        public async Task AddAsync(Asset asset)
            => await context.Assets.AddAsync(asset);

        public Task UpdateAsync(Asset asset)
        {
            context.Assets.Update(asset);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Asset asset)
        {
            context.Assets.Remove(asset);
            return Task.CompletedTask;
        }
    }
}
