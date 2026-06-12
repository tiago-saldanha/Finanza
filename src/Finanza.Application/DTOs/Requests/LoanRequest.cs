namespace Finanza.Application.DTOs.Requests
{
    public record LoanRequest(
        string   BorrowerName,
        decimal  TotalAmount,
        DateTime StartDate,
        DateTime DueDate,
        string?  Notes,
        int      InstallmentCount
    );

    public record PayInstallmentRequest(DateTime PaidAt);
}
