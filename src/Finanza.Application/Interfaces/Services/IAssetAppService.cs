using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IAssetAppService
    {
        Task<IEnumerable<AssetResponse>> GetAllAsync();
        Task<AssetResponse>              GetByIdAsync(Guid id);
        Task<AssetResponse>              CreateAsync(AssetRequest request);
        Task<AssetResponse>              UpdateAsync(Guid id, AssetRequest request);
        Task                             DeleteAsync(Guid id);
    }
}
