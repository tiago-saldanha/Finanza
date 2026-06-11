using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.PatrimonySnapshotAppServiceTests
{
    public class PatrimonySnapshotAppServiceTests
    {
        private readonly Mock<IPatrimonySnapshotRepository> _snapshotRepo  = new();
        private readonly Mock<IAssetRepository>             _assetRepo     = new();
        private readonly Mock<ILiabilityRepository>         _liabilityRepo = new();
        private readonly Mock<IUnitOfWork>                  _unitOfWork    = new();
        private readonly PatrimonySnapshotAppService        _service;

        public PatrimonySnapshotAppServiceTests()
            => _service = new PatrimonySnapshotAppService(
                _snapshotRepo.Object, _assetRepo.Object,
                _liabilityRepo.Object, _unitOfWork.Object);

        [Fact]
        public async Task CreateAsync_ShouldCaptureCurrentNetWorth()
        {
            _assetRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                Asset.Create("Imóvel", AssetType.Property, 300_000),
                Asset.Create("Carro",  AssetType.Vehicle,   50_000),
            ]);
            _liabilityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                Liability.Create("Financiamento", LiabilityType.Financing, 150_000),
            ]);

            var result = await _service.CreateAsync();

            _snapshotRepo.Verify(r => r.AddAsync(It.IsAny<PatrimonySnapshot>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal(350_000, result.TotalAssets);
            Assert.Equal(150_000, result.TotalLiabilities);
            Assert.Equal(200_000, result.NetWorth);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSnapshotsOrderedByDate()
        {
            var snapshots = new List<PatrimonySnapshot>
            {
                PatrimonySnapshot.Create(new DateTime(2025, 3, 1), 200_000, 100_000),
                PatrimonySnapshot.Create(new DateTime(2025, 1, 1), 150_000,  80_000),
                PatrimonySnapshot.Create(new DateTime(2025, 2, 1), 180_000,  90_000),
            };
            _snapshotRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(snapshots);

            var result = (await _service.GetAllAsync()).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(new DateTime(2025, 1, 1), result[0].Date);
            Assert.Equal(new DateTime(2025, 2, 1), result[1].Date);
            Assert.Equal(new DateTime(2025, 3, 1), result[2].Date);
        }

        [Fact]
        public async Task CreateAsync_WhenNoData_ShouldCreateZeroSnapshot()
        {
            _assetRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _liabilityRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.CreateAsync();

            Assert.Equal(0, result.TotalAssets);
            Assert.Equal(0, result.TotalLiabilities);
            Assert.Equal(0, result.NetWorth);
        }
    }
}
