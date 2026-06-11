using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Exceptions;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class AssetAppService(IAssetRepository repository, IUnitOfWork unitOfWork) : IAssetAppService
    {
        public async Task<IEnumerable<AssetResponse>> GetAllAsync()
        {
            var assets = await repository.GetAllAsync();
            return assets.Select(AssetResponse.Create);
        }

        public async Task<AssetResponse> GetByIdAsync(Guid id)
        {
            var asset = await repository.GetByIdAsync(id);
            return AssetResponse.Create(asset);
        }

        public async Task<AssetResponse> CreateAsync(AssetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new AssetNameAppException();
            var asset = Asset.Create(request.Name, request.Type, request.Value);
            await repository.AddAsync(asset);
            await unitOfWork.CommitAsync();
            return AssetResponse.Create(asset);
        }

        public async Task<AssetResponse> UpdateAsync(Guid id, AssetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new AssetNameAppException();
            var asset = await repository.GetByIdAsync(id);
            asset.Update(request.Name, request.Type, request.Value);
            await repository.UpdateAsync(asset);
            await unitOfWork.CommitAsync();
            return AssetResponse.Create(asset);
        }

        public async Task DeleteAsync(Guid id)
        {
            var asset = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(asset);
            await unitOfWork.CommitAsync();
        }
    }
}
