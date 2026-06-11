using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class InvestmentResponse
    {
        public Guid    Id             { get; init; }
        public string  Name           { get; init; } = "";
        public string  Type           { get; init; } = "";
        public decimal InvestedAmount { get; init; }
        public decimal CurrentValue   { get; init; }
        public decimal Return         { get; init; }
        public decimal ReturnRate     { get; init; }

        public static InvestmentResponse Create(Investment i) => new()
        {
            Id             = i.Id,
            Name           = i.Name.Value,
            Type           = i.Type.ToString(),
            InvestedAmount = i.InvestedAmount.Value,
            CurrentValue   = i.CurrentValue.Value,
            Return         = i.Return,
            ReturnRate     = Math.Round(i.ReturnRate, 2),
        };
    }

    public class InvestmentPortfolioResponse
    {
        public IEnumerable<InvestmentResponse> Investments    { get; init; } = [];
        public decimal TotalInvested  { get; init; }
        public decimal TotalCurrent   { get; init; }
        public decimal TotalReturn    { get; init; }
        public decimal TotalReturnRate { get; init; }
        public IEnumerable<InvestmentTypeAllocation> Allocations { get; init; } = [];
    }

    public class InvestmentTypeAllocation
    {
        public string  Type       { get; init; } = "";
        public decimal Value      { get; init; }
        public decimal Percentage { get; init; }
    }
}
