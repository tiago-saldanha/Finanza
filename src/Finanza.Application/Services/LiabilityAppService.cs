using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Exceptions;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class LiabilityAppService(ILiabilityRepository repository, IUnitOfWork unitOfWork) : ILiabilityAppService
    {
        public async Task<IEnumerable<LiabilityResponse>> GetAllAsync()
        {
            var liabilities = await repository.GetAllAsync();
            return liabilities.Select(LiabilityResponse.Create);
        }

        public async Task<LiabilityResponse> GetByIdAsync(Guid id)
        {
            var liability = await repository.GetByIdAsync(id);
            return LiabilityResponse.Create(liability);
        }

        public async Task<LiabilityResponse> CreateAsync(LiabilityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new LiabilityNameAppException();
            var liability = Liability.Create(request.Name, request.Type, request.Value);
            await repository.AddAsync(liability);
            await unitOfWork.CommitAsync();
            return LiabilityResponse.Create(liability);
        }

        public async Task<LiabilityResponse> UpdateAsync(Guid id, LiabilityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new LiabilityNameAppException();
            var liability = await repository.GetByIdAsync(id);
            liability.Update(request.Name, request.Type, request.Value);
            await repository.UpdateAsync(liability);
            await unitOfWork.CommitAsync();
            return LiabilityResponse.Create(liability);
        }

        public async Task DeleteAsync(Guid id)
        {
            var liability = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(liability);
            await unitOfWork.CommitAsync();
        }
    }
}
