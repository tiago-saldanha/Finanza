using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class LoanPayableInstallment
    {
        protected LoanPayableInstallment() { }

        private LoanPayableInstallment(Guid id, Guid loanPayableId, int number, Money amount, DateTime dueDate)
        {
            Id            = id;
            LoanPayableId = loanPayableId;
            Number        = number;
            Amount        = amount;
            DueDate       = dueDate.Date;
        }

        public static LoanPayableInstallment Create(Guid loanPayableId, int number, decimal amount, DateTime dueDate)
            => new(Guid.NewGuid(), loanPayableId, number, new Money(amount), dueDate);

        public Guid     Id            { get; private set; }
        public Guid     LoanPayableId { get; private set; }
        public virtual  LoanPayable LoanPayable { get; private set; } = null!;
        public int      Number        { get; private set; }
        public Money    Amount        { get; private set; }
        public DateTime DueDate       { get; private set; }
        public DateTime? PaidAt       { get; private set; }

        public bool IsPaid    => PaidAt.HasValue;
        public bool IsOverdue => !IsPaid && DateTime.Today > DueDate;

        public void Pay(DateTime paidAt)
        {
            if (IsPaid) return;
            PaidAt = paidAt;
        }

        public void Unpay() => PaidAt = null;
    }
}
