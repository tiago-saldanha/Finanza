using Finanza.Application.DTOs.Requests;
using Finanza.Application.Exceptions;
using Finanza.Application.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Moq;

namespace Finanza.Application.Tests.Services.GoalAppServiceTests
{
    public class GoalAppServiceTests
    {
        private readonly Mock<IGoalRepository> _repo       = new();
        private readonly Mock<IUnitOfWork>     _unitOfWork = new();
        private readonly GoalAppService        _service;

        public GoalAppServiceTests()
            => _service = new GoalAppService(_repo.Object, _unitOfWork.Object);

        [Fact]
        public async Task CreateAsync_ShouldReturnGoalWithProgress()
        {
            var req    = new GoalRequest("Viagem", 10_000, 2_500, new DateTime(2026, 12, 31));
            var result = await _service.CreateAsync(req);

            _repo.Verify(r => r.AddAsync(It.IsAny<Goal>()), Times.Once);
            Assert.Equal("Viagem", result.Name);
            Assert.Equal(25,       result.ProgressRate);
            Assert.Equal(7_500,    result.Remaining);
            Assert.False(result.IsCompleted);
        }

        [Fact]
        public async Task CreateAsync_WithEmptyName_ShouldThrow()
            => await Assert.ThrowsAsync<GoalNameAppException>(()
                => _service.CreateAsync(new GoalRequest("", 1000, 0, DateTime.Today)));

        [Fact]
        public async Task ContributeAsync_ShouldUpdateCurrentAmount()
        {
            var goal = Goal.Create("Reserva", 5_000, 1_000, DateTime.Today.AddYears(1));
            _repo.Setup(r => r.GetByIdAsync(goal.Id)).ReturnsAsync(goal);

            var result = await _service.ContributeAsync(goal.Id, new ContributeGoalRequest(500));

            Assert.Equal(1_500, result.CurrentAmount);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ContributeAsync_ShouldNotExceedTarget()
        {
            var goal = Goal.Create("Fundo", 1_000, 900, DateTime.Today.AddYears(1));
            _repo.Setup(r => r.GetByIdAsync(goal.Id)).ReturnsAsync(goal);

            var result = await _service.ContributeAsync(goal.Id, new ContributeGoalRequest(500));

            Assert.Equal(1_000, result.CurrentAmount);
            Assert.True(result.IsCompleted);
        }
    }
}
