using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class TransactionResponse
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public string Status { get; private set; }
        public string Type { get; private set; }
        public string CategoryName { get; private set; }
        public bool IsOverdue { get; private set; }
        public Guid? AccountId { get; private set; }
        public string? AccountName { get; private set; }
        public Guid? DestinationAccountId { get; private set; }
        public string? DestinationAccountName { get; private set; }
        public Guid? AssetId { get; private set; }
        public string? AssetName { get; private set; }
        public Guid? LiabilityId { get; private set; }
        public string? LiabilityName { get; private set; }
        public Guid? LoanReceivableId { get; private set; }
        public string? LoanReceivableName { get; private set; }

        public static TransactionResponse Create(Transaction transaction)
        {
            return new TransactionResponse
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                DueDate = transaction.Dates.DueDate,
                CreatedAt = transaction.Dates.CreatedAt,
                PaymentDate = transaction.PaymentDate,
                Status = transaction.Status.ToString(),
                Type = transaction.Type.ToString(),
                CategoryName = transaction.Category?.Name ?? string.Empty,
                IsOverdue = transaction.IsOverdue(DateTime.Today),
                AccountId = transaction.AccountId,
                AccountName = transaction.Account?.Name,
                DestinationAccountId = transaction.DestinationAccountId,
                DestinationAccountName = transaction.DestinationAccount?.Name,
                AssetId = transaction.AssetId,
                AssetName = transaction.Asset?.Name,
                LiabilityId = transaction.LiabilityId,
                LiabilityName = transaction.Liability?.Name,
                LoanReceivableId = transaction.LoanReceivableId,
                LoanReceivableName = transaction.LoanReceivable?.BorrowerName,
            };
        }
    }
}
