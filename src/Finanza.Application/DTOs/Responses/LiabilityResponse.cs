using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class LiabilityInstallmentResponse
    {
        public Guid      Id        { get; init; }
        public int       Number    { get; init; }
        public decimal   Amount    { get; init; }
        public DateTime  DueDate   { get; init; }
        public DateTime? PaidAt    { get; init; }
        public bool      IsPaid    { get; init; }
        public bool      IsOverdue { get; init; }

        public static LiabilityInstallmentResponse Create(LiabilityInstallment i) => new()
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

    public class LiabilityResponse
    {
        public Guid      Id               { get; init; }
        public string    Name             { get; init; } = default!;
        public string    Type             { get; init; } = default!;
        public decimal   Value            { get; init; }
        public DateTime? StartDate        { get; init; }
        public DateTime? DueDate          { get; init; }
        public string?   Notes            { get; init; }
        public decimal   TotalPaid        { get; init; }
        public decimal   Balance          { get; init; }
        public bool      IsSettled        { get; init; }
        public bool      HasOverdue       { get; init; }
        public int       InstallmentCount { get; init; }
        public int       PaidCount        { get; init; }
        public IEnumerable<LiabilityInstallmentResponse> Installments { get; init; } = [];

        public static LiabilityResponse Create(Liability liability) => new()
        {
            Id               = liability.Id,
            Name             = liability.Name,
            Type             = liability.Type.ToString(),
            Value            = liability.Value,
            StartDate        = liability.StartDate,
            DueDate          = liability.DueDate,
            Notes            = liability.Notes,
            TotalPaid        = liability.TotalPaid,
            Balance          = liability.Balance,
            IsSettled        = liability.IsSettled,
            HasOverdue       = liability.HasOverdue,
            InstallmentCount = liability.Installments.Count,
            PaidCount        = liability.Installments.Count(i => i.IsPaid),
            Installments     = liability.Installments.OrderBy(i => i.Number).Select(LiabilityInstallmentResponse.Create),
        };
    }
}
