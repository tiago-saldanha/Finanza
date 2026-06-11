using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.FinancialPlanningAppServiceTests
{
    public class FinancialPlanningAppServiceTests
    {
        private readonly Mock<ITransactionRepository> _txRepo = new();
        private readonly Mock<IFinancialAccountRepository> _accRepo = new();
        private readonly FinancialPlanningAppService _service;

        public FinancialPlanningAppServiceTests()
        {
            _service = new FinancialPlanningAppService(_txRepo.Object, _accRepo.Object);
        }

        private static Transaction MakePaidTx(TransactionType type, decimal amount, int year, int month)
        {
            var catId = Guid.NewGuid();
            var due   = new DateTime(year, month, 15);
            var tx    = Transaction.Create("tx", amount, due, type, catId, due.AddDays(-1));
            tx.Pay(due);
            return tx;
        }

        [Fact]
        public async Task GetAsync_WhenRevenueAndExpenses_ShouldComputeSavingsRate()
        {
            var year = 2025; var month = 1;

            _txRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                MakePaidTx(TransactionType.Revenue, 5000, year, month),
                MakePaidTx(TransactionType.Expense, 3000, year, month),
            ]);
            _accRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAsync(year, month);

            Assert.Equal(5000,  result.MonthlyRevenue);
            Assert.Equal(3000,  result.MonthlyExpenses);
            Assert.Equal(2000,  result.MonthlySavings);
            Assert.Equal(40.0m, result.SavingsRate);    // (5000-3000)/5000*100
            Assert.Equal(60.0m, result.ExpenseRatio);   // 3000/5000*100
        }

        [Fact]
        public async Task GetAsync_WhenNoRevenue_ShouldReturnZeroRates()
        {
            _txRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _accRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAsync(2025, 1);

            Assert.Equal(0, result.SavingsRate);
            Assert.Equal(0, result.ExpenseRatio);
        }

        [Fact]
        public async Task GetAsync_WithAccountBalances_ShouldComputeEmergencyFund()
        {
            var year = 2025; var month = 4;

            // Despesas médias dos últimos 3 meses: 1000+2000+3000 / 3 = 2000
            var transactions = new List<Transaction>
            {
                MakePaidTx(TransactionType.Expense, 1000, year, month - 3),
                MakePaidTx(TransactionType.Expense, 2000, year, month - 2),
                MakePaidTx(TransactionType.Expense, 3000, year, month - 1),
            };
            _txRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

            // Conta com saldo inicial 12000 (sem transações)
            var account = Account.Create("Poupança", AccountType.Savings, 12000);
            _accRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([account]);

            var result = await _service.GetAsync(year, month);

            Assert.Equal(12000, result.TotalAccountBalance);
            Assert.Equal(2000,  result.AvgMonthlyExpenses);
            Assert.Equal(6.0m,  result.EmergencyFundMonths); // 12000 / 2000
        }
    }
}
