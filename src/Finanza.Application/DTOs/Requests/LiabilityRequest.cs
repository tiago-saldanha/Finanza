using Finanza.Domain.Enums;

namespace Finanza.Application.DTOs.Requests
{
    public record LiabilityRequest(
        string Name,
        LiabilityType Type,
        decimal Value,
        DateTime? StartDate = null,
        DateTime? DueDate = null,
        string? Notes = null,
        int InstallmentCount = 0
    );

    public record PayLiabilityInstallmentRequest(DateTime PaidAt);
}
