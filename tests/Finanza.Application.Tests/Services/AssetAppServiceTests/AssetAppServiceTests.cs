using Finanza.Application.DTOs.Requests;
using Finanza.Application.Exceptions;
using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.AssetAppServiceTests
{
    public class AssetAppServiceTests
    {
        private readonly Mock<IAssetRepository> _repo      = new();
        private readonly Mock<IUnitOfWork>      _unitOfWork = new();
        private readonly AssetAppService        _service;

        public AssetAppServiceTests()
            => _service = new AssetAppService(_repo.Object, _unitOfWork.Object);

        [Fact]
        public async Task CreateAsync_WhenValid_ShouldCreate()
        {
            var request = new AssetRequest("Imóvel", AssetType.Property, 300_000);

            var result = await _service.CreateAsync(request);

            _repo.Verify(r => r.AddAsync(It.IsAny<Asset>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal("Imóvel",   result.Name);
            Assert.Equal("Property", result.Type);
            Assert.Equal(300_000,    result.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WhenNameInvalid_ShouldThrow(string? name)
        {
            await Assert.ThrowsAsync<AssetNameAppException>(
                () => _service.CreateAsync(new AssetRequest(name!, AssetType.Other, 0)));

            _repo.Verify(r => r.AddAsync(It.IsAny<Asset>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenValid_ShouldUpdate()
        {
            var asset = Asset.Create("Carro", AssetType.Vehicle, 50_000);
            _repo.Setup(r => r.GetByIdAsync(asset.Id)).ReturnsAsync(asset);

            var result = await _service.UpdateAsync(asset.Id,
                new AssetRequest("Carro Novo", AssetType.Vehicle, 60_000));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Asset>()), Times.Once);
            Assert.Equal("Carro Novo", result.Name);
            Assert.Equal(60_000,       result.Value);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete()
        {
            var asset = Asset.Create("Carro", AssetType.Vehicle, 50_000);
            _repo.Setup(r => r.GetByIdAsync(asset.Id)).ReturnsAsync(asset);

            await _service.DeleteAsync(asset.Id);

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Asset>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAll()
        {
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                Asset.Create("Imóvel",  AssetType.Property, 300_000),
                Asset.Create("Carro",   AssetType.Vehicle,   50_000),
            ]);

            var result = await _service.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }
}
