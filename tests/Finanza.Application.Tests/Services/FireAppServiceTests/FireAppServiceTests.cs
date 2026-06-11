using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.FireAppServiceTests
{
    public class FireAppServiceTests
    {
        private readonly Mock<ITransactionRepository> _txRepo  = new();
        private readonly Mock<IInvestmentRepository>  _invRepo = new();
        private readonly FireAppService               _service;

        private static DateTime Due(int monthsAgo)
            => DateTime.UtcNow.AddMonths(-monthsAgo);

        private static Transaction MakePaidTx(decimal amount, TransactionType type, int monthsAgo)
        {
            var due = Due(monthsAgo);
            var tx  = Transaction.Create("tx", amount, due, type, Guid.NewGuid(), due.AddDays(-1));
            tx.Pay(due);
            return tx;
        }

        public FireAppServiceTests()
            => _service = new FireAppService(_txRepo.Object, _invRepo.Object);

        [Fact]
        public async Task GetAsync_WithNoData_ShouldReturnZeros()
        {
            _txRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _invRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAsync();

            Assert.Equal(0, result.AnnualExpenses);
            Assert.Equal(0, result.FireGoal);
            Assert.Equal(0, result.TotalInvested);
            Assert.Equal(-1, result.EstimatedYearsToFire);
        }

        [Fact]
        public async Task GetAsync_ShouldComputeFireGoalAs25xAnnualExpenses()
        {
            // R$1000/mês de despesa → anual = 12k → FIRE = 300k
            _txRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                MakePaidTx(2_000, TransactionType.Revenue,  0),
                MakePaidTx(1_000, TransactionType.Expense,  0),
            ]);
            _invRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAsync();

            Assert.Equal(12_000, result.AnnualExpenses);
            Assert.Equal(300_000, result.FireGoal);
            Assert.Equal(50, result.SavingsRate);
        }

        [Fact]
        public async Task GetAsync_WhenAlreadyFIRE_ShouldReturnZeroYears()
        {
            _txRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                MakePaidTx(1_000, TransactionType.Expense, 0),
            ]);
            _invRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                Investment.Create("Carteira", InvestmentType.RendaFixa, 300_000, 500_000),
            ]);

            var result = await _service.GetAsync();

            Assert.Equal(0, result.EstimatedYearsToFire);
            Assert.Equal(100, result.ProgressRate);
        }
    }
}
