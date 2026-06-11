using Finanza.Domain.Enums;

namespace Finanza.Application.DTOs.Requests
{
    public record FinancialAccountRequest(
        string Name,
        AccountType Type,
        decimal InitialBalance
    );
}
