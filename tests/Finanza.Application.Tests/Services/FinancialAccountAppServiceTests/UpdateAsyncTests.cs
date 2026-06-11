using Finanza.Application.DTOs.Requests;
using Finanza.Application.Exceptions;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums; // AccountType needed for Account.Create and request
using Moq;

namespace Finanza.Application.Tests.Services.FinancialAccountAppServiceTests
{
    public class UpdateAsyncTests : FinancialAccountAppServiceBaseTests
    {
        [Fact]
        public async Task UpdateAsync_WhenRequestIsValid_ShouldUpdateAccount()
        {
            var account = Account.Create("Conta Corrente", AccountType.Checking, 1000);
            _repositoryMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var request = new FinancialAccountRequest("Conta Atualizada", AccountType.Savings, 2000);
            var result = await _service.UpdateAsync(account.Id, request);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal("Conta Atualizada", result.Name);
            Assert.Equal("Savings",          result.Type);
            Assert.Equal(2000,               result.InitialBalance);
        }

        [Theory]
        [InlineData("    ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateAsync_WhenNameIsInvalid_ShouldThrowAccountNameAppException(string? invalidName)
        {
            var account = Account.Create("Conta Corrente", AccountType.Checking, 1000);
            _repositoryMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var request = new FinancialAccountRequest(invalidName!, AccountType.Checking, 0);

            await Assert.ThrowsAsync<AccountNameAppException>(() => _service.UpdateAsync(account.Id, request));
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Never);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
