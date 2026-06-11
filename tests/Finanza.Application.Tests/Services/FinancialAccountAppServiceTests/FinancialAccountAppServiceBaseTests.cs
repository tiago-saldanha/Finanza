using Finanza.Application.Services;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.FinancialAccountAppServiceTests
{
    public class FinancialAccountAppServiceBaseTests
    {
        protected readonly Mock<IFinancialAccountRepository> _repositoryMock;
        protected readonly Mock<IUnitOfWork> _unitOfWork;
        protected readonly FinancialAccountAppService _service;

        public FinancialAccountAppServiceBaseTests()
        {
            _repositoryMock = new Mock<IFinancialAccountRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _service = new FinancialAccountAppService(_repositoryMock.Object, _unitOfWork.Object);
        }
    }
}
