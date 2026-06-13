using Finanza.Application.DTOs.Responses;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class FinancialPlanningAppService(
        ITransactionRepository transactionRepository,
        IFinancialAccountRepository accountRepository) : IFinancialPlanningAppService
    {
        public async Task<FinancialPlanningResponse> GetAsync(int year, int month)
        {
            var allTransactions = await transactionRepository.GetAllAsync();
            var allAccounts     = await accountRepository.GetAllAsync();

            // ── Mês atual ────────────────────────────────────────────────
            var monthTx = allTransactions
                .Where(t => t.Status == TransactionStatus.Paid
                         && t.Dates.DueDate.Year == year
                         && t.Dates.DueDate.Month == month)
                .ToList();

            var revenue  = monthTx.Where(t => t.Type == TransactionType.Revenue  && t.Type != TransactionType.Transfer).Sum(t => t.Amount.Value);
            var expenses = monthTx.Where(t => t.Type == TransactionType.Expense  && t.Type != TransactionType.Transfer).Sum(t => t.Amount.Value);
            var savings  = revenue - expenses;

            var savingsRate  = revenue > 0 ? savings  / revenue * 100 : 0;
            var expenseRatio = revenue > 0 ? expenses / revenue * 100 : 0;

            // ── Média mensal de despesas (últimos 3 meses) ────────────────
            var refDate     = new DateTime(year, month, 1);
            var threeMonths = Enumerable.Range(1, 3)
                .Select(i => refDate.AddMonths(-i))
                .ToList();

            var avgMonthlyExpenses = threeMonths.Count == 0 ? expenses :
                threeMonths.Average(m =>
                    allTransactions
                        .Where(t => t.Status == TransactionStatus.Paid
                                 && t.Type == TransactionType.Expense
                                 && t.Type != TransactionType.Transfer
                                 && t.Dates.DueDate.Year == m.Year
                                 && t.Dates.DueDate.Month == m.Month)
                        .Sum(t => t.Amount.Value));

            // ── Saldo total das contas ────────────────────────────────────
            var totalBalance = allAccounts.Sum(a =>
            {
                var paid = a.Transactions.Where(t => t.Status == TransactionStatus.Paid);
                return a.InitialBalance.Value
                    + paid.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.Amount.Value)
                    - paid.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount.Value)
                    - paid.Where(t => t.Type == TransactionType.Transfer).Sum(t => t.Amount.Value)
                    + a.IncomingTransfers.Where(t => t.Status == TransactionStatus.Paid).Sum(t => t.Amount.Value);
            });

            var emergencyMonths = avgMonthlyExpenses > 0
                ? Math.Round(totalBalance / avgMonthlyExpenses, 1)
                : 0;

            return new FinancialPlanningResponse
            {
                Year                = year,
                Month               = month,
                MonthlyRevenue      = Math.Round(revenue,           2),
                MonthlyExpenses     = Math.Round(expenses,          2),
                MonthlySavings      = Math.Round(savings,           2),
                SavingsRate         = Math.Round(savingsRate,       1),
                SavingsRatio        = Math.Round(savingsRate,       1),
                ExpenseRatio        = Math.Round(expenseRatio,      1),
                TotalAccountBalance = Math.Round(totalBalance,      2),
                AvgMonthlyExpenses  = Math.Round(avgMonthlyExpenses,2),
                EmergencyFundMonths = emergencyMonths,
            };
        }
    }
}
