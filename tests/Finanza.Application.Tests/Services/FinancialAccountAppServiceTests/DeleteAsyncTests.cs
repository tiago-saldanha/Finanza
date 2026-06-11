using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Moq;

namespace Finanza.Application.Tests.Services.FinancialAccountAppServiceTests
{
    public class DeleteAsyncTests : FinancialAccountAppServiceBaseTests
    {
        [Fact]
        public async Task DeleteAsync_WhenAccountExists_ShouldDelete()
        {
            var account = Account.Create("Conta Corrente", AccountType.Checking, 1000);
            _repositoryMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            await _service.DeleteAsync(account.Id);

            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Account>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
