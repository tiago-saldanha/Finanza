using Finanza.Domain.Enums;
using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class Investment
    {
        protected Investment() { }

        private Investment(Guid id, Description name, InvestmentType type, Money investedAmount, Money currentValue)
        {
            Id             = id;
            Name           = name;
            Type           = type;
            InvestedAmount = investedAmount;
            CurrentValue   = currentValue;
        }

        public static Investment Create(string name, InvestmentType type, decimal investedAmount, decimal currentValue)
            => new(Guid.NewGuid(), new Description(name), type, new Money(investedAmount), new Money(currentValue));

        public void Update(string name, InvestmentType type, decimal investedAmount, decimal currentValue)
        {
            Name           = new Description(name);
            Type           = type;
            InvestedAmount = new Money(investedAmount);
            CurrentValue   = new Money(currentValue);
        }

        public Guid           Id             { get; private set; }
        public Description    Name           { get; private set; }
        public InvestmentType Type           { get; private set; }
        public Money          InvestedAmount { get; private set; }
        public Money          CurrentValue   { get; private set; }

        // Computed
        public decimal Return     => CurrentValue.Value - InvestedAmount.Value;
        public decimal ReturnRate => InvestedAmount.Value == 0 ? 0
            : (Return / InvestedAmount.Value) * 100;
    }
}
