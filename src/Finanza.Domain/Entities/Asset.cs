using Finanza.Domain.Enums;
using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class Asset
    {
        protected Asset() { }

        private Asset(Guid id, Description name, AssetType type, Money value)
        {
            Id    = id;
            Name  = name;
            Type  = type;
            Value = value;
        }

        public static Asset Create(string name, AssetType type, decimal value)
            => new(Guid.NewGuid(), new Description(name), type, new Money(value));

        public void Update(string name, AssetType type, decimal value)
        {
            Name  = new Description(name);
            Type  = type;
            Value = new Money(value);
        }

        public Guid      Id    { get; private set; }
        public Description Name  { get; private set; }
        public AssetType Type  { get; private set; }
        public Money     Value { get; private set; }
    }
}
