using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class LoanInstallment
    {
        protected LoanInstallment() { }

        private LoanInstallment(Guid id, Guid loanReceivableId, int number, Money amount, DateTime dueDate)
        {
            Id                = id;
            LoanReceivableId  = loanReceivableId;
            Number            = number;
            Amount            = amount;
            DueDate           = dueDate.Date;
        }

        public static LoanInstallment Create(Guid loanReceivableId, int number, decimal amount, DateTime dueDate)
            => new(Guid.NewGuid(), loanReceivableId, number, new Money(amount), dueDate);

        public Guid     Id               { get; private set; }
        public Guid     LoanReceivableId { get; private set; }
        public virtual  LoanReceivable LoanReceivable { get; private set; } = null!;
        public int      Number           { get; private set; }
        public Money    Amount           { get; private set; }
        public DateTime DueDate          { get; private set; }
        public DateTime? PaidAt          { get; private set; }

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
