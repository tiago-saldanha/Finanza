using Finanza.Application.DTOs.Responses;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class FireAppService(
        ITransactionRepository  transactionRepository,
        IInvestmentRepository   investmentRepository) : IFireAppService
    {
        public async Task<FireResponse> GetAsync()
        {
            var transactions = await transactionRepository.GetAllAsync();
            var investments  = await investmentRepository.GetAllAsync();

            var paid = transactions.Where(t => t.Status == TransactionStatus.Paid).ToList();

            // Média mensal de despesas dos últimos 12 meses
            var now = DateTime.UtcNow;
            var last12 = Enumerable.Range(0, 12)
                .Select(i => now.AddMonths(-i))
                .ToList();

            var monthlyExpenses = last12.Select(m =>
                paid.Where(t => t.Type == TransactionType.Expense
                             && t.Dates.DueDate.Year  == m.Year
                             && t.Dates.DueDate.Month == m.Month)
                    .Sum(t => t.Amount.Value)).ToList();

            var avgMonthlyExpenses = monthlyExpenses.Any(e => e > 0)
                ? monthlyExpenses.Where(e => e > 0).Average()
                : 0;

            var annualExpenses = Math.Round(avgMonthlyExpenses * 12, 2);
            var fireGoal       = Math.Round(annualExpenses * 25, 2);  // regra dos 4%

            // Poupança mensal média (últimos 12 meses)
            var monthlyRevenues = last12.Select(m =>
                paid.Where(t => t.Type == TransactionType.Revenue
                             && t.Dates.DueDate.Year  == m.Year
                             && t.Dates.DueDate.Month == m.Month)
                    .Sum(t => t.Amount.Value)).ToList();

            var avgRevenue = monthlyRevenues.Any(r => r > 0)
                ? monthlyRevenues.Where(r => r > 0).Average()
                : 0;

            var monthlySavings = Math.Round(avgRevenue - avgMonthlyExpenses, 2);
            var savingsRate    = avgRevenue > 0
                ? Math.Round(monthlySavings / avgRevenue * 100, 1)
                : 0;

            // Total investido (valor atual da carteira)
            var totalInvested = investments.Sum(i => i.CurrentValue.Value);
            var progressRate  = fireGoal > 0
                ? Math.Round(Math.Min(totalInvested / fireGoal * 100, 100), 2)
                : 0;

            // Anos até FIRE: (FireGoal - TotalInvested) / (MonthlySavings * 12)
            decimal yearsToFire = -1;
            var remaining = fireGoal - totalInvested;
            if (fireGoal > 0 && remaining <= 0)
                yearsToFire = 0;
            else if (fireGoal > 0 && monthlySavings > 0)
                yearsToFire = Math.Round(remaining / (monthlySavings * 12), 1);

            return new FireResponse
            {
                AnnualExpenses       = annualExpenses,
                FireGoal             = fireGoal,
                TotalInvested        = Math.Round(totalInvested, 2),
                ProgressRate         = progressRate,
                MonthlySavings       = monthlySavings,
                SavingsRate          = savingsRate,
                EstimatedYearsToFire = yearsToFire,
            };
        }
    }
}
