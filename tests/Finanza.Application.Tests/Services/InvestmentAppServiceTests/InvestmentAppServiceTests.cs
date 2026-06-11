using Finanza.Application.DTOs.Requests;
using Finanza.Application.Exceptions;
using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.InvestmentAppServiceTests
{
    public class InvestmentAppServiceTests
    {
        private readonly Mock<IInvestmentRepository> _repo       = new();
        private readonly Mock<IUnitOfWork>           _unitOfWork = new();
        private readonly InvestmentAppService        _service;

        public InvestmentAppServiceTests()
            => _service = new InvestmentAppService(_repo.Object, _unitOfWork.Object);

        [Fact]
        public async Task CreateAsync_ShouldReturnInvestment()
        {
            var req    = new InvestmentRequest("Tesouro Selic", InvestmentType.RendaFixa, 10_000, 10_500);
            var result = await _service.CreateAsync(req);

            _repo.Verify(r => r.AddAsync(It.IsAny<Investment>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal("Tesouro Selic", result.Name);
            Assert.Equal("RendaFixa",     result.Type);
            Assert.Equal(10_000,          result.InvestedAmount);
            Assert.Equal(10_500,          result.CurrentValue);
            Assert.Equal(500,             result.Return);
            Assert.Equal(5,               result.ReturnRate);
        }

        [Fact]
        public async Task CreateAsync_WithEmptyName_ShouldThrow()
        {
            var req = new InvestmentRequest("", InvestmentType.Acoes, 1000, 1000);
            await Assert.ThrowsAsync<InvestmentNameAppException>(() => _service.CreateAsync(req));
        }

        [Fact]
        public async Task GetPortfolioAsync_ShouldComputeKPIs()
        {
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                Investment.Create("Tesouro", InvestmentType.RendaFixa, 10_000, 10_500),
                Investment.Create("PETR4",   InvestmentType.Acoes,      5_000,  4_500),
            ]);

            var portfolio = await _service.GetPortfolioAsync();

            Assert.Equal(15_000, portfolio.TotalInvested);
            Assert.Equal(15_000, portfolio.TotalCurrent);
            Assert.Equal(0,      portfolio.TotalReturn);
            Assert.Equal(2,      portfolio.Investments.Count());
            Assert.Equal(2,      portfolio.Allocations.Count());
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemove()
        {
            var inv = Investment.Create("FII XPLG11", InvestmentType.FII, 2000, 2100);
            _repo.Setup(r => r.GetByIdAsync(inv.Id)).ReturnsAsync(inv);

            await _service.DeleteAsync(inv.Id);

            _repo.Verify(r => r.DeleteAsync(inv), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
