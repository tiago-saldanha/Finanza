using Finanza.Domain.Enums;
using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class Account
    {
        protected Account() { }

        private Account(Guid id, Description name, AccountType type, Money initialBalance)
        {
            Id = id;
            Name = name;
            Type = type;
            InitialBalance = initialBalance;
        }

        public static Account Create(string name, AccountType type, decimal initialBalance)
        {
            return new Account(
                Guid.NewGuid(),
                new Description(name),
                type,
                new Money(initialBalance)
            );
        }

        public void Update(string name, AccountType type, decimal initialBalance)
        {
            Name = new Description(name);
            Type = type;
            InitialBalance = new Money(initialBalance);
        }

        public Guid Id { get; private set; }
        public Description Name { get; private set; }
        public AccountType Type { get; private set; }
        public Money InitialBalance { get; private set; }
        public virtual ICollection<Transaction> Transactions { get; set; } = [];
    }
}
