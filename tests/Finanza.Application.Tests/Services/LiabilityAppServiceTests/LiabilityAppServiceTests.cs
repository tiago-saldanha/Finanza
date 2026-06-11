using Finanza.Application.DTOs.Requests;
using Finanza.Application.Exceptions;
using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.LiabilityAppServiceTests
{
    public class LiabilityAppServiceTests
    {
        private readonly Mock<ILiabilityRepository> _repo      = new();
        private readonly Mock<IUnitOfWork>           _unitOfWork = new();
        private readonly LiabilityAppService         _service;

        public LiabilityAppServiceTests()
            => _service = new LiabilityAppService(_repo.Object, _unitOfWork.Object);

        [Fact]
        public async Task CreateAsync_WhenValid_ShouldCreate()
        {
            var request = new LiabilityRequest("Financiamento Imóvel", LiabilityType.Financing, 150_000);

            var result = await _service.CreateAsync(request);

            _repo.Verify(r => r.AddAsync(It.IsAny<Liability>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal("Financiamento Imóvel", result.Name);
            Assert.Equal("Financing",             result.Type);
            Assert.Equal(150_000,                 result.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WhenNameInvalid_ShouldThrow(string? name)
        {
            await Assert.ThrowsAsync<LiabilityNameAppException>(
                () => _service.CreateAsync(new LiabilityRequest(name!, LiabilityType.Other, 0)));

            _repo.Verify(r => r.AddAsync(It.IsAny<Liability>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete()
        {
            var liability = Liability.Create("Empréstimo", LiabilityType.Loan, 10_000);
            _repo.Setup(r => r.GetByIdAsync(liability.Id)).ReturnsAsync(liability);

            await _service.DeleteAsync(liability.Id);

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Liability>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
