using Finanza.Domain.Entities;
using Finanza.Domain.Enums;

namespace Finanza.Application.DTOs.Responses
{
    public class FinancialAccountResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Type { get; init; } = default!;
        public decimal InitialBalance { get; init; }
        public decimal CurrentBalance { get; init; }

        public static FinancialAccountResponse Create(Account account)
        {
            var paid = account.Transactions.Where(t => t.Status == TransactionStatus.Paid);

            var revenues        = paid.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.Amount.Value);
            var expenses        = paid.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount.Value);
            var transfersOut    = paid.Where(t => t.Type == TransactionType.Transfer).Sum(t => t.Amount.Value);
            var transfersIn     = account.IncomingTransfers.Where(t => t.Status == TransactionStatus.Paid).Sum(t => t.Amount.Value);

            return new FinancialAccountResponse
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type.ToString(),
                InitialBalance = account.InitialBalance,
                CurrentBalance = account.InitialBalance.Value + revenues - expenses - transfersOut + transfersIn,
            };
        }
    }
}
