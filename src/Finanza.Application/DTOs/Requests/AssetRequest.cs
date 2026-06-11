using Finanza.Domain.Enums;

namespace Finanza.Application.DTOs.Requests
{
    public record AssetRequest(string Name, AssetType Type, decimal Value);
}
