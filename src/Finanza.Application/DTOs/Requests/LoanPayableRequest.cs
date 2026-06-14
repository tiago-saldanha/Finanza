namespace Finanza.Application.DTOs.Requests
{
    public record LoanPayableRequest(
        string   CreditorName,
        decimal  TotalAmount,
        DateTime StartDate,
        DateTime DueDate,
        string?  Notes,
        int      InstallmentCount
    );

    public record PayPayableInstallmentRequest(DateTime PaidAt);
}
