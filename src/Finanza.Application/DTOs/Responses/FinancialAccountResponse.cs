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
            var paidRevenues = account.Transactions
                .Where(t => t.Status == TransactionStatus.Paid && t.Type == TransactionType.Revenue)
                .Sum(t => t.Amount.Value);

            var paidExpenses = account.Transactions
                .Where(t => t.Status == TransactionStatus.Paid && t.Type == TransactionType.Expense)
                .Sum(t => t.Amount.Value);

            return new FinancialAccountResponse
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type.ToString(),
                InitialBalance = account.InitialBalance,
                CurrentBalance = account.InitialBalance.Value + paidRevenues - paidExpenses,
            };
        }
    }
}
