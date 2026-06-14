using Finanza.Domain.Abstractions;
using Finanza.Domain.Enums;
using Finanza.Domain.Events;
using Finanza.Domain.Exceptions;
using Finanza.Domain.ValueObjects;

namespace Finanza.Domain.Entities
{
    public class Transaction : Entity
    {
        protected Transaction() { }

        private Transaction(Guid id, Description description, Money amount, TransactionDates dates, TransactionType type, Guid? categoryId, Guid? accountId, Guid? destinationAccountId, Guid? assetId, Guid? liabilityId, Guid? loanReceivableId, Guid? loanPayableId, Guid? investmentId, Guid? goalId)
        {
            Id = id;
            Description = description;
            Amount = amount;
            Dates = dates;
            Type = type;
            CategoryId = categoryId;
            AccountId = accountId;
            DestinationAccountId = destinationAccountId;
            AssetId = assetId;
            LiabilityId = liabilityId;
            LoanReceivableId = loanReceivableId;
            LoanPayableId = loanPayableId;
            InvestmentId = investmentId;
            GoalId = goalId;
            Status = TransactionStatus.Pending;
        }

        public static Transaction Create(string description, decimal amount, DateTime dueDate, TransactionType type, Guid? categoryId, DateTime createdAt, Guid? accountId = null, Guid? destinationAccountId = null, Guid? assetId = null, Guid? liabilityId = null, Guid? loanReceivableId = null, Guid? loanPayableId = null, Guid? investmentId = null, Guid? goalId = null)
        {
            return new Transaction(
                Guid.NewGuid(),
                new Description(description),
                new Money(amount),
                new TransactionDates(dueDate, createdAt),
                type,
                categoryId,
                accountId,
                destinationAccountId,
                assetId,
                liabilityId,
                loanReceivableId,
                loanPayableId,
                investmentId,
                goalId
            );
        }

        public Guid Id { get; private set; }
        public Description Description { get; private set; }
        public Money Amount { get; private set; }
        public TransactionDates Dates { get; set; }
        public DateTime? PaymentDate { get; private set; }
        public TransactionStatus Status { get; private set; }
        public TransactionType Type { get; private set; }
        public Guid? CategoryId { get; private set; }
        public virtual Category? Category { get; private set; }
        public Guid? AccountId { get; private set; }
        public virtual Account? Account { get; private set; }
        public Guid? DestinationAccountId { get; private set; }
        public virtual Account? DestinationAccount { get; private set; }
        public Guid? AssetId { get; private set; }
        public virtual Asset? Asset { get; private set; }
        public Guid? LiabilityId { get; private set; }
        public virtual Liability? Liability { get; private set; }
        public Guid? LoanReceivableId { get; private set; }
        public virtual LoanReceivable? LoanReceivable { get; private set; }
        public Guid? LoanPayableId { get; private set; }
        public virtual LoanPayable? LoanPayable { get; private set; }
        public Guid? InvestmentId { get; private set; }
        public virtual Investment? Investment { get; private set; }
        public Guid? GoalId { get; private set; }
        public virtual Goal? Goal { get; private set; }

        public void Pay(DateTime paymentDate)
        {
            if (Status == TransactionStatus.Cancelled)
                throw new TransactionPayException("A transação já foi cancelada");

            if (Status == TransactionStatus.Paid)
                throw new TransactionPayException("A transação já foi paga");

            //if (paymentDate.Date < Dates.CreatedAt.Date)
            //    throw new TransactionPayException("A data de pagamento não pode ser anterior à data de criação da transação");

            PaymentDate = paymentDate;
            Status = TransactionStatus.Paid;

            AddDomainEvent(new TransactionPaidEvent(Id, paymentDate));
        }

        public void Reopen()
        {
            if (Status != TransactionStatus.Paid)
                throw new TransactionReopenException();

            Status = TransactionStatus.Pending;
            PaymentDate = null;

            AddDomainEvent(new TransactionReopenEvent(Id, Status));
        }

        public void Cancel()
        {
            if (Status == TransactionStatus.Cancelled)
                throw new TransactionCancelException("Não é possível cancelar uma transação que já foi cancelada");

            Status = TransactionStatus.Cancelled;
            PaymentDate = null;

            AddDomainEvent(new TransactionCancelEvent(Id, Status));
        }

        public void Update(string description, decimal amount, DateTime dueDate, TransactionType type, Guid? categoryId, Guid? accountId = null, Guid? destinationAccountId = null, Guid? assetId = null, Guid? liabilityId = null, Guid? loanReceivableId = null, Guid? loanPayableId = null, Guid? investmentId = null, Guid? goalId = null)
        {
            if (Status == TransactionStatus.Cancelled)
                throw new TransactionUpdateException("Não é possível editar uma transação que foi cancelada");

            Description = new Description(description);
            Amount = new Money(amount);
            Dates = new TransactionDates(dueDate, Dates.CreatedAt);
            Type = type;
            CategoryId = categoryId;
            AccountId = accountId;
            DestinationAccountId = destinationAccountId;
            AssetId = assetId;
            LiabilityId = liabilityId;
            LoanReceivableId = loanReceivableId;
            LoanPayableId = loanPayableId;
            InvestmentId = investmentId;
            GoalId = goalId;
        }

        public bool IsOverdue(DateTime today) => Status == TransactionStatus.Pending && today.Date > Dates.DueDate.Date;
    }
}
