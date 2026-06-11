using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class PatrimonySnapshotResponse
    {
        public Guid     Id               { get; init; }
        public DateTime Date             { get; init; }
        public decimal  TotalAssets      { get; init; }
        public decimal  TotalLiabilities { get; init; }
        public decimal  NetWorth         { get; init; }

        public static PatrimonySnapshotResponse Create(PatrimonySnapshot s) => new()
        {
            Id               = s.Id,
            Date             = s.Date,
            TotalAssets      = s.TotalAssets.Value,
            TotalLiabilities = s.TotalLiabilities.Value,
            NetWorth         = s.NetWorth,
        };
    }
}
