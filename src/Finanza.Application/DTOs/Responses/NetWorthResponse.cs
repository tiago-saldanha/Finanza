namespace Finanza.Application.DTOs.Responses
{
    public class NetWorthResponse
    {
        public decimal TotalAssets      { get; init; }
        public decimal TotalInvestments { get; init; }
        public decimal TotalLiabilities { get; init; }
        public decimal NetWorth         { get; init; }  // TotalAssets + TotalInvestments - TotalLiabilities

        public IEnumerable<AssetResponse>      Assets      { get; init; } = [];
        public IEnumerable<LiabilityResponse>  Liabilities { get; init; } = [];
        public IEnumerable<InvestmentResponse> Investments { get; init; } = [];
    }
}
