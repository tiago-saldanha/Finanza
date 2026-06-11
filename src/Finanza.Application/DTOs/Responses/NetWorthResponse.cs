namespace Finanza.Application.DTOs.Responses
{
    public class NetWorthResponse
    {
        public decimal TotalAssets      { get; init; }
        public decimal TotalLiabilities { get; init; }
        public decimal NetWorth         { get; init; }  // TotalAssets - TotalLiabilities

        public IEnumerable<AssetResponse>     Assets      { get; init; } = [];
        public IEnumerable<LiabilityResponse> Liabilities { get; init; } = [];
    }
}
