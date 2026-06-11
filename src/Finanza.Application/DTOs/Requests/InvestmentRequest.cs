using Finanza.Domain.Enums;

namespace Finanza.Application.DTOs.Requests
{
    public record InvestmentRequest(string Name, InvestmentType Type, decimal InvestedAmount, decimal CurrentValue);
}
