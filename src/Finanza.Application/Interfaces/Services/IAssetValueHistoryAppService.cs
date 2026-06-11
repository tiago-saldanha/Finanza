using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IAssetValueHistoryAppService
    {
        Task<IEnumerable<AssetValueHistoryResponse>> GetByAssetIdAsync(Guid assetId);
        Task<AssetValueHistoryResponse> UpdateValueAsync(Guid assetId, UpdateAssetValueRequest request);
    }
}
