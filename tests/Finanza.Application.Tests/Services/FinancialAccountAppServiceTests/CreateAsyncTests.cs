using Finanza.Application.DTOs.Requests;
using Finanza.Application.Exceptions;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Moq;

namespace Finanza.Application.Tests.Services.FinancialAccountAppServiceTests
{
    public class CreateAsyncTests : FinancialAccountAppServiceBaseTests
    {
        [Theory]
        [InlineData("Conta Corrente", AccountType.Checking, 1000)]
        [InlineData("Carteira",       AccountType.Wallet,   0)]
        [InlineData("Poupança",       AccountType.Savings,  500.50)]
        public async Task CreateAsync_WhenRequestIsValid_ShouldCreateAccount(string name, AccountType type, decimal balance)
        {
            var request = new FinancialAccountRequest(name, type, balance);

            var result = await _service.CreateAsync(request);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal(name,           result.Name);
            Assert.Equal(type.ToString(), result.Type);
            Assert.Equal(balance,        result.InitialBalance);
        }

        [Theory]
        [InlineData("    ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateAsync_WhenNameIsInvalid_ShouldThrowAccountNameAppException(string? invalidName)
        {
            var request = new FinancialAccountRequest(invalidName!, AccountType.Checking, 0);

            await Assert.ThrowsAsync<AccountNameAppException>(() => _service.CreateAsync(request));
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Never);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
