using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Exceptions;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class InvestmentAppService(IInvestmentRepository repository, IUnitOfWork unitOfWork) : IInvestmentAppService
    {
        public async Task<InvestmentPortfolioResponse> GetPortfolioAsync()
        {
            var investments = (await repository.GetAllAsync()).ToList();
            var responses   = investments.Select(InvestmentResponse.Create).ToList();

            var totalInvested = investments.Sum(i => i.InvestedAmount.Value);
            var totalCurrent  = investments.Sum(i => i.CurrentValue.Value);
            var totalReturn   = totalCurrent - totalInvested;
            var returnRate    = totalInvested == 0 ? 0 : Math.Round(totalReturn / totalInvested * 100, 2);

            var allocations = investments
                .GroupBy(i => i.Type)
                .Select(g => new InvestmentTypeAllocation
                {
                    Type       = g.Key.ToString(),
                    Value      = g.Sum(i => i.CurrentValue.Value),
                    Percentage = totalCurrent == 0 ? 0
                        : Math.Round(g.Sum(i => i.CurrentValue.Value) / totalCurrent * 100, 2),
                });

            return new InvestmentPortfolioResponse
            {
                Investments     = responses,
                TotalInvested   = totalInvested,
                TotalCurrent    = totalCurrent,
                TotalReturn     = totalReturn,
                TotalReturnRate = returnRate,
                Allocations     = allocations,
            };
        }

        public async Task<InvestmentResponse> GetByIdAsync(Guid id)
            => InvestmentResponse.Create(await repository.GetByIdAsync(id));

        public async Task<InvestmentResponse> CreateAsync(InvestmentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new InvestmentNameAppException();
            var investment = Investment.Create(request.Name, request.Type, request.InvestedAmount, request.CurrentValue);
            await repository.AddAsync(investment);
            await unitOfWork.CommitAsync();
            return InvestmentResponse.Create(investment);
        }

        public async Task<InvestmentResponse> UpdateAsync(Guid id, InvestmentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new InvestmentNameAppException();
            var investment = await repository.GetByIdAsync(id);
            investment.Update(request.Name, request.Type, request.InvestedAmount, request.CurrentValue);
            await repository.UpdateAsync(investment);
            await unitOfWork.CommitAsync();
            return InvestmentResponse.Create(investment);
        }

        public async Task DeleteAsync(Guid id)
        {
            var investment = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(investment);
            await unitOfWork.CommitAsync();
        }
    }
}
