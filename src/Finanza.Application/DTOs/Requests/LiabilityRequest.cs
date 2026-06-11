using Finanza.Domain.Enums;

namespace Finanza.Application.DTOs.Requests
{
    public record LiabilityRequest(string Name, LiabilityType Type, decimal Value);
}
