using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class LiabilityInstallment
    {
        protected LiabilityInstallment() { }

        private LiabilityInstallment(Guid id, Guid liabilityId, int number, Money amount, DateTime dueDate)
        {
            Id          = id;
            LiabilityId = liabilityId;
            Number      = number;
            Amount      = amount;
            DueDate     = dueDate.Date;
        }

        public static LiabilityInstallment Create(Guid liabilityId, int number, decimal amount, DateTime dueDate)
            => new(Guid.NewGuid(), liabilityId, number, new Money(amount), dueDate);

        public Guid      Id          { get; private set; }
        public Guid      LiabilityId { get; private set; }
        public virtual   Liability Liability { get; private set; } = null!;
        public int       Number      { get; private set; }
        public Money     Amount      { get; private set; }
        public DateTime  DueDate     { get; private set; }
        public DateTime? PaidAt      { get; private set; }

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
