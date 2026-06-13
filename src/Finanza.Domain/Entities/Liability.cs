using Finanza.Domain.Enums;
using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class Liability
    {
        protected Liability() { }

        private Liability(Guid id, Description name, LiabilityType type, Money value, DateTime? startDate, DateTime? dueDate, string? notes)
        {
            Id        = id;
            Name      = name;
            Type      = type;
            Value     = value;
            StartDate = startDate?.Date;
            DueDate   = dueDate?.Date;
            Notes     = notes;
        }

        public static Liability Create(string name, LiabilityType type, decimal value, DateTime? startDate = null, DateTime? dueDate = null, string? notes = null, int installmentCount = 0)
        {
            var liability = new Liability(Guid.NewGuid(), new Description(name), type, new Money(value), startDate, dueDate, notes);

            if (installmentCount > 0 && startDate.HasValue)
            {
                var installmentAmount = Math.Round(value / installmentCount, 2);
                for (int i = 1; i <= installmentCount; i++)
                {
                    var installmentDue = startDate.Value.AddMonths(i);
                    liability.Installments.Add(LiabilityInstallment.Create(liability.Id, i, installmentAmount, installmentDue));
                }
            }

            return liability;
        }

        public void Update(string name, LiabilityType type, decimal value, DateTime? startDate = null, DateTime? dueDate = null, string? notes = null)
        {
            Name      = new Description(name);
            Type      = type;
            Value     = new Money(value);
            StartDate = startDate?.Date;
            DueDate   = dueDate?.Date;
            Notes     = notes;
        }

        public Guid          Id        { get; private set; }
        public Description   Name      { get; private set; }
        public LiabilityType Type      { get; private set; }
        public Money         Value     { get; private set; }
        public DateTime?     StartDate { get; private set; }
        public DateTime?     DueDate   { get; private set; }
        public string?       Notes     { get; private set; }

        public virtual ICollection<LiabilityInstallment> Installments { get; private set; } = [];

        public decimal TotalPaid  => Installments.Where(i => i.IsPaid).Sum(i => i.Amount.Value);
        public decimal Balance    => Value.Value - TotalPaid;
        public bool    IsSettled  => Balance <= 0;
        public bool    HasOverdue => Installments.Any(i => i.IsOverdue);
    }
}
