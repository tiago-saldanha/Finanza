namespace Finanza.Application.DTOs.Requests
{
    public class UpdateTransactionRequest
    {
        public string Description { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string TransactionType { get; set; } = default!;
        public Guid? CategoryId { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public Guid? AssetId { get; set; }
        public Guid? LiabilityId { get; set; }
        public Guid? LoanReceivableId { get; set; }
        public Guid? LoanPayableId { get; set; }
        public Guid? InvestmentId { get; set; }
        public Guid? GoalId { get; set; }
    }
}
