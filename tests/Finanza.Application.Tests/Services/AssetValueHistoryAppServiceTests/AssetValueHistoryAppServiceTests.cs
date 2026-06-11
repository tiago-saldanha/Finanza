using Finanza.Application.DTOs.Requests;
using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.AssetValueHistoryAppServiceTests
{
    public class AssetValueHistoryAppServiceTests
    {
        private readonly Mock<IAssetValueHistoryRepository> _historyRepo = new();
        private readonly Mock<IAssetRepository>             _assetRepo   = new();
        private readonly Mock<IUnitOfWork>                  _unitOfWork  = new();
        private readonly AssetValueHistoryAppService        _service;

        public AssetValueHistoryAppServiceTests()
            => _service = new AssetValueHistoryAppService(
                _historyRepo.Object, _assetRepo.Object, _unitOfWork.Object);

        [Fact]
        public async Task UpdateValueAsync_ShouldUpdateAssetAndAddHistory()
        {
            var asset = Asset.Create("Imóvel", AssetType.Property, 300_000);
            _assetRepo.Setup(r => r.GetByIdAsync(asset.Id)).ReturnsAsync(asset);

            var request = new UpdateAssetValueRequest(320_000, DateTime.Today);
            var result  = await _service.UpdateValueAsync(asset.Id, request);

            _assetRepo.Verify(r => r.UpdateAsync(asset), Times.Once);
            _historyRepo.Verify(r => r.AddAsync(It.IsAny<AssetValueHistory>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal(320_000, result.Value);
            Assert.Equal(asset.Id, result.AssetId);
        }

        [Fact]
        public async Task GetByAssetIdAsync_ShouldReturnHistoryOrderedByDate()
        {
            var assetId = Guid.NewGuid();
            var history = new List<AssetValueHistory>
            {
                AssetValueHistory.Create(assetId, new DateTime(2025, 3, 1), 310_000),
                AssetValueHistory.Create(assetId, new DateTime(2025, 1, 1), 290_000),
                AssetValueHistory.Create(assetId, new DateTime(2025, 2, 1), 300_000),
            };
            _historyRepo.Setup(r => r.GetByAssetIdAsync(assetId)).ReturnsAsync(history);

            var result = (await _service.GetByAssetIdAsync(assetId)).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(new DateTime(2025, 1, 1), result[0].Date);
            Assert.Equal(new DateTime(2025, 2, 1), result[1].Date);
            Assert.Equal(new DateTime(2025, 3, 1), result[2].Date);
        }

        [Fact]
        public async Task GetByAssetIdAsync_WhenNoHistory_ShouldReturnEmpty()
        {
            var assetId = Guid.NewGuid();
            _historyRepo.Setup(r => r.GetByAssetIdAsync(assetId)).ReturnsAsync([]);

            var result = await _service.GetByAssetIdAsync(assetId);

            Assert.Empty(result);
        }
    }
}
