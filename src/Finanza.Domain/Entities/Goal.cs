using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class Goal
    {
        protected Goal() { }

        private Goal(Guid id, Description name, Money targetAmount, Money currentAmount, DateTime targetDate)
        {
            Id            = id;
            Name          = name;
            TargetAmount  = targetAmount;
            CurrentAmount = currentAmount;
            TargetDate    = targetDate.Date;
        }

        public static Goal Create(string name, decimal targetAmount, decimal currentAmount, DateTime targetDate)
            => new(Guid.NewGuid(), new Description(name), new Money(targetAmount), new Money(currentAmount), targetDate);

        public void Update(string name, decimal targetAmount, decimal currentAmount, DateTime targetDate)
        {
            Name          = new Description(name);
            TargetAmount  = new Money(targetAmount);
            CurrentAmount = new Money(currentAmount);
            TargetDate    = targetDate.Date;
        }

        public void Contribute(decimal amount)
        {
            var newAmount = CurrentAmount.Value + amount;
            CurrentAmount = new Money(Math.Min(newAmount, TargetAmount.Value));
        }

        public Guid        Id            { get; private set; }
        public Description Name          { get; private set; }
        public Money       TargetAmount  { get; private set; }
        public Money       CurrentAmount { get; private set; }
        public DateTime    TargetDate    { get; private set; }

        public decimal  ProgressRate => TargetAmount.Value == 0 ? 0
            : Math.Min(Math.Round(CurrentAmount.Value / TargetAmount.Value * 100, 2), 100);
        public decimal  Remaining    => Math.Max(TargetAmount.Value - CurrentAmount.Value, 0);
        public bool     IsCompleted  => CurrentAmount.Value >= TargetAmount.Value;
    }
}
