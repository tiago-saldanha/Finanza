using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class AssetResponse
    {
        public Guid    Id    { get; init; }
        public string  Name  { get; init; } = default!;
        public string  Type  { get; init; } = default!;
        public decimal Value { get; init; }

        public static AssetResponse Create(Asset asset) => new()
        {
            Id    = asset.Id,
            Name  = asset.Name,
            Type  = asset.Type.ToString(),
            Value = asset.Value,
        };
    }
}
