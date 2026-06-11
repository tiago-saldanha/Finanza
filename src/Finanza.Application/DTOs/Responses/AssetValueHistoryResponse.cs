using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class AssetValueHistoryResponse
    {
        public Guid     Id      { get; init; }
        public Guid     AssetId { get; init; }
        public DateTime Date    { get; init; }
        public decimal  Value   { get; init; }

        public static AssetValueHistoryResponse Create(AssetValueHistory h)
            => new() { Id = h.Id, AssetId = h.AssetId, Date = h.Date, Value = h.Value.Value };
    }
}
