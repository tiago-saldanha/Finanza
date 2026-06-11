using Finanza.Application.DTOs.Responses;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class PatrimonySnapshotAppService(
        IPatrimonySnapshotRepository snapshotRepository,
        IAssetRepository             assetRepository,
        ILiabilityRepository         liabilityRepository,
        IUnitOfWork                  unitOfWork) : IPatrimonySnapshotAppService
    {
        public async Task<IEnumerable<PatrimonySnapshotResponse>> GetAllAsync()
        {
            var snapshots = await snapshotRepository.GetAllAsync();
            return snapshots
                .OrderBy(s => s.Date)
                .Select(PatrimonySnapshotResponse.Create);
        }

        public async Task<PatrimonySnapshotResponse> CreateAsync()
        {
            var assets      = await assetRepository.GetAllAsync();
            var liabilities = await liabilityRepository.GetAllAsync();

            var totalAssets      = assets.Sum(a => a.Value.Value);
            var totalLiabilities = liabilities.Sum(l => l.Value.Value);

            var snapshot = PatrimonySnapshot.Create(DateTime.UtcNow, totalAssets, totalLiabilities);

            await snapshotRepository.AddAsync(snapshot);
            await unitOfWork.CommitAsync();

            return PatrimonySnapshotResponse.Create(snapshot);
        }
    }
}
