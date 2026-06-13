using Finanza.Domain.Entities;
using Finanza.Domain.Enums;

namespace Finanza.Application.DTOs.Requests
{
    public class CreateTransactionRequest
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string TransactionType { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public Guid? AssetId { get; set; }
        public Guid? LiabilityId { get; set; }
        public Guid? LoanReceivableId { get; set; }

        public Transaction ToEntity()
        {
            return Transaction.Create(
                Description,
                Amount,
                DueDate,
                Map(TransactionType),
                CategoryId,
                CreatedAt,
                AccountId,
                DestinationAccountId,
                AssetId,
                LiabilityId,
                LoanReceivableId
            );
        }

        private TransactionType Map(string transactionType) => Mapper.Mapper.TransactionType(transactionType);
    }
}
