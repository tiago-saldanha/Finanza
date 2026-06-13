using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.NetWorthAppServiceTests
{
    public class NetWorthAppServiceTests
    {
        private readonly Mock<IAssetRepository>      _assetRepo      = new();
        private readonly Mock<ILiabilityRepository>  _liabilityRepo  = new();
        private readonly Mock<IInvestmentRepository> _investmentRepo = new();
        private readonly NetWorthAppService          _service;

        public NetWorthAppServiceTests()
        {
            _investmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _service = new NetWorthAppService(_assetRepo.Object, _liabilityRepo.Object, _investmentRepo.Object);
        }

        [Fact]
        public async Task GetAsync_ShouldComputeNetWorth()
        {
            _assetRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                Asset.Create("Imóvel", AssetType.Property,    300_000),
                Asset.Create("Carro",  AssetType.Vehicle,      50_000),
            ]);
            _liabilityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                Liability.Create("Financiamento", LiabilityType.Financing, 150_000),
            ]);

            var result = await _service.GetAsync();

            Assert.Equal(350_000, result.TotalAssets);
            Assert.Equal(150_000, result.TotalLiabilities);
            Assert.Equal(200_000, result.NetWorth);
            Assert.Equal(2, result.Assets.Count());
            Assert.Single(result.Liabilities);
        }

        [Fact]
        public async Task GetAsync_WhenNoData_ShouldReturnZero()
        {
            _assetRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _liabilityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAsync();

            Assert.Equal(0, result.NetWorth);
        }
    }
}
