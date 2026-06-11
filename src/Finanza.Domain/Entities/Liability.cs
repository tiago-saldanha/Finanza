using Finanza.Domain.Enums;
using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class Liability
    {
        protected Liability() { }

        private Liability(Guid id, Description name, LiabilityType type, Money value)
        {
            Id    = id;
            Name  = name;
            Type  = type;
            Value = value;
        }

        public static Liability Create(string name, LiabilityType type, decimal value)
            => new(Guid.NewGuid(), new Description(name), type, new Money(value));

        public void Update(string name, LiabilityType type, decimal value)
        {
            Name  = new Description(name);
            Type  = type;
            Value = new Money(value);
        }

        public Guid          Id    { get; private set; }
        public Description   Name  { get; private set; }
        public LiabilityType Type  { get; private set; }
        public Money         Value { get; private set; }
    }
}
