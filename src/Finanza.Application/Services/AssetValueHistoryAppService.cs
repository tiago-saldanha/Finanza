using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class AssetValueHistoryAppService(
        IAssetValueHistoryRepository historyRepository,
        IAssetRepository             assetRepository,
        IUnitOfWork                  unitOfWork) : IAssetValueHistoryAppService
    {
        public async Task<IEnumerable<AssetValueHistoryResponse>> GetByAssetIdAsync(Guid assetId)
        {
            var history = await historyRepository.GetByAssetIdAsync(assetId);
            return history.OrderBy(h => h.Date).Select(AssetValueHistoryResponse.Create);
        }

        public async Task<AssetValueHistoryResponse> UpdateValueAsync(Guid assetId, UpdateAssetValueRequest request)
        {
            var asset = await assetRepository.GetByIdAsync(assetId);
            asset.UpdateValue(request.Value);
            await assetRepository.UpdateAsync(asset);

            var entry = AssetValueHistory.Create(assetId, request.Date, request.Value);
            await historyRepository.AddAsync(entry);
            await unitOfWork.CommitAsync();

            return AssetValueHistoryResponse.Create(entry);
        }
    }
}
