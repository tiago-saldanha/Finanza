using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Moq;

namespace Finanza.Application.Tests.Services.FinancialAccountAppServiceTests
{
    public class GetByIdAsyncTests : FinancialAccountAppServiceBaseTests
    {
        [Fact]
        public async Task GetByIdAsync_WhenAccountExists_ShouldReturnAccount()
        {
            var account = Account.Create("Conta Corrente", AccountType.Checking, 1000);
            _repositoryMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _service.GetByIdAsync(account.Id);

            Assert.Equal(account.Name.Value, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenAccountDoesNotExist_ShouldThrow()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Conta não encontrada"));

            await Assert.ThrowsAsync<Exception>(
                () => _service.GetByIdAsync(Guid.NewGuid()));
        }
    }
}
