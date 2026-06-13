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
            // Para passivos com parcelas, usa o saldo devedor (Balance); sem parcelas, usa o valor cheio
            var totalLiabilities = liabilities.Sum(l => l.Installments.Count > 0 ? l.Balance : l.Value.Value);

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
