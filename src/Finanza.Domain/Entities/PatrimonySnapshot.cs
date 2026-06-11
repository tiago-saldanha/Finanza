using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class PatrimonySnapshot
    {
        protected PatrimonySnapshot() { }

        private PatrimonySnapshot(Guid id, DateTime date, Money totalAssets, Money totalLiabilities)
        {
            Id               = id;
            Date             = date.Date; // normaliza para meia-noite
            TotalAssets      = totalAssets;
            TotalLiabilities = totalLiabilities;
        }

        public static PatrimonySnapshot Create(DateTime date, decimal totalAssets, decimal totalLiabilities)
            => new(Guid.NewGuid(), date, new Money(totalAssets), new Money(totalLiabilities));

        public Guid  Id               { get; private set; }
        public DateTime Date          { get; private set; }
        public Money TotalAssets      { get; private set; }
        public Money TotalLiabilities { get; private set; }
        public decimal NetWorth       => TotalAssets.Value - TotalLiabilities.Value;
    }
}
