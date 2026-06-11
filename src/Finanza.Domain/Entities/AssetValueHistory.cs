using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class AssetValueHistory
    {
        protected AssetValueHistory() { }

        private AssetValueHistory(Guid id, Guid assetId, DateTime date, Money value)
        {
            Id      = id;
            AssetId = assetId;
            Date    = date.Date;
            Value   = value;
        }

        public static AssetValueHistory Create(Guid assetId, DateTime date, decimal value)
            => new(Guid.NewGuid(), assetId, date, new Money(value));

        public Guid     Id      { get; private set; }
        public Guid     AssetId { get; private set; }
        public DateTime Date    { get; private set; }
        public Money    Value   { get; private set; }

        public Asset? Asset { get; private set; }
    }
}
