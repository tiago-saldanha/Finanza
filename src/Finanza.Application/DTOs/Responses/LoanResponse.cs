using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class LoanInstallmentResponse
    {
        public Guid      Id       { get; init; }
        public int       Number   { get; init; }
        public decimal   Amount   { get; init; }
        public DateTime  DueDate  { get; init; }
        public DateTime? PaidAt   { get; init; }
        public bool      IsPaid   { get; init; }
        public bool      IsOverdue { get; init; }

        public static LoanInstallmentResponse Create(LoanInstallment i) => new()
        {
            Id        = i.Id,
            Number    = i.Number,
            Amount    = i.Amount.Value,
            DueDate   = i.DueDate,
            PaidAt    = i.PaidAt,
            IsPaid    = i.IsPaid,
            IsOverdue = i.IsOverdue,
        };
    }

    public class LoanResponse
    {
        public Guid     Id               { get; init; }
        public string   BorrowerName     { get; init; } = "";
        public decimal  TotalAmount      { get; init; }
        public decimal  TotalPaid        { get; init; }
        public decimal  Balance          { get; init; }
        public DateTime StartDate        { get; init; }
        public DateTime DueDate          { get; init; }
        public string?  Notes            { get; init; }
        public bool     IsSettled        { get; init; }
        public bool     HasOverdue       { get; init; }
        public int      InstallmentCount { get; init; }
        public int      PaidCount        { get; init; }
        public IEnumerable<LoanInstallmentResponse> Installments { get; init; } = [];

        public static LoanResponse Create(LoanReceivable l) => new()
        {
            Id               = l.Id,
            BorrowerName     = l.BorrowerName.Value,
            TotalAmount      = l.TotalAmount.Value,
            TotalPaid        = l.TotalPaid,
            Balance          = l.Balance,
            StartDate        = l.StartDate,
            DueDate          = l.DueDate,
            Notes            = l.Notes,
            IsSettled        = l.IsSettled,
            HasOverdue       = l.HasOverdue,
            InstallmentCount = l.Installments.Count,
            PaidCount        = l.Installments.Count(i => i.IsPaid),
            Installments     = l.Installments.OrderBy(i => i.Number).Select(LoanInstallmentResponse.Create),
        };
    }

    public class LoanSummaryResponse
    {
        public decimal TotalLoaned   { get; init; }
        public decimal TotalReceived { get; init; }
        public decimal TotalBalance  { get; init; }
        public int     OverdueCount  { get; init; }

        public static LoanSummaryResponse Create(IEnumerable<LoanReceivable> loans)
        {
            var list = loans.ToList();
            return new LoanSummaryResponse
            {
                TotalLoaned   = list.Sum(l => l.TotalAmount.Value),
                TotalReceived = list.Sum(l => l.TotalPaid),
                TotalBalance  = list.Sum(l => l.Balance),
                OverdueCount  = list.Sum(l => l.Installments.Count(i => i.IsOverdue)),
            };
        }
    }
}
