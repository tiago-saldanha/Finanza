namespace Finanza.Domain.Repositories
{
    public interface IAssetValueHistoryRepository
    {
        Task<IEnumerable<Entities.AssetValueHistory>> GetByAssetIdAsync(Guid assetId);
        Task AddAsync(Entities.AssetValueHistory history);
    }
}
