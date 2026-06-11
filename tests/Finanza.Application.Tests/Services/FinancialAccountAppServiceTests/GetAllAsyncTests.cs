using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Moq;

namespace Finanza.Application.Tests.Services.FinancialAccountAppServiceTests
{
    public class GetAllAsyncTests : FinancialAccountAppServiceBaseTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAccounts()
        {
            var accounts = new List<Account>
            {
                Account.Create("Conta Corrente", AccountType.Checking, 1000),
                Account.Create("Carteira",       AccountType.Wallet,   0),
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(accounts);

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenNoAccounts_ShouldReturnEmpty()
        {
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAllAsync();

            Assert.Empty(result);
        }
    }
}
