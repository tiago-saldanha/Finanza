using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class LiabilityResponse
    {
        public Guid    Id    { get; init; }
        public string  Name  { get; init; } = default!;
        public string  Type  { get; init; } = default!;
        public decimal Value { get; init; }

        public static LiabilityResponse Create(Liability liability) => new()
        {
            Id    = liability.Id,
            Name  = liability.Name,
            Type  = liability.Type.ToString(),
            Value = liability.Value,
        };
    }
}
