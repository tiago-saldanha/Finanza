using Finanza.Application.DTOs.Responses;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class NetWorthAppService(IAssetRepository assetRepository, ILiabilityRepository liabilityRepository) : INetWorthAppService
    {
        public async Task<NetWorthResponse> GetAsync()
        {
            var assets      = (await assetRepository.GetAllAsync()).ToList();
            var liabilities = (await liabilityRepository.GetAllAsync()).ToList();

            var totalAssets      = assets.Sum(a => a.Value.Value);
            var totalLiabilities = liabilities.Sum(l => l.Value.Value);

            return new NetWorthResponse
            {
                TotalAssets      = Math.Round(totalAssets,      2),
                TotalLiabilities = Math.Round(totalLiabilities, 2),
                NetWorth         = Math.Round(totalAssets - totalLiabilities, 2),
                Assets           = assets.Select(AssetResponse.Create),
                Liabilities      = liabilities.Select(LiabilityResponse.Create),
            };
        }
    }
}
