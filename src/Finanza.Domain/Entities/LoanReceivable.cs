using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class LoanReceivable
    {
        protected LoanReceivable() { }

        private LoanReceivable(Guid id, Description borrowerName, Money totalAmount, DateTime startDate, DateTime dueDate, string? notes)
        {
            Id           = id;
            BorrowerName = borrowerName;
            TotalAmount  = totalAmount;
            StartDate    = startDate.Date;
            DueDate      = dueDate.Date;
            Notes        = notes;
        }

        public static LoanReceivable Create(string borrowerName, decimal totalAmount, DateTime startDate, DateTime dueDate, string? notes, int installmentCount)
        {
            var loan = new LoanReceivable(
                Guid.NewGuid(),
                new Description(borrowerName),
                new Money(totalAmount),
                startDate,
                dueDate,
                notes
            );

            var installmentAmount = Math.Round(totalAmount / installmentCount, 2);

            for (int i = 1; i <= installmentCount; i++)
            {
                var installmentDue = startDate.AddMonths(i);
                loan.Installments.Add(LoanInstallment.Create(loan.Id, i, installmentAmount, installmentDue));
            }

            return loan;
        }

        public void Update(string borrowerName, decimal totalAmount, DateTime startDate, DateTime dueDate, string? notes)
        {
            BorrowerName = new Description(borrowerName);
            TotalAmount  = new Money(totalAmount);
            StartDate    = startDate.Date;
            DueDate      = dueDate.Date;
            Notes        = notes;
        }

        public Guid        Id           { get; private set; }
        public Description BorrowerName { get; private set; }
        public Money       TotalAmount  { get; private set; }
        public DateTime    StartDate    { get; private set; }
        public DateTime    DueDate      { get; private set; }
        public string?     Notes        { get; private set; }

        public virtual ICollection<LoanInstallment> Installments { get; private set; } = [];

        public decimal TotalPaid    => Installments.Where(i => i.IsPaid).Sum(i => i.Amount.Value);
        public decimal Balance      => TotalAmount.Value - TotalPaid;
        public bool    IsSettled    => Balance <= 0;
        public bool    HasOverdue   => Installments.Any(i => i.IsOverdue);
    }
}
