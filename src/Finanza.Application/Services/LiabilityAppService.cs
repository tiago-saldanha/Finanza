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
            var liability = Liability.Create(request.Name, request.Type, request.Value, request.StartDate, request.DueDate, request.Notes, request.InstallmentCount);
            await repository.AddAsync(liability);
            await unitOfWork.CommitAsync();
            return LiabilityResponse.Create(liability);
        }

        public async Task<LiabilityResponse> UpdateAsync(Guid id, LiabilityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new LiabilityNameAppException();
            var liability = await repository.GetByIdAsync(id);
            liability.Update(request.Name, request.Type, request.Value, request.StartDate, request.DueDate, request.Notes);
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

        public async Task<LiabilityInstallmentResponse> PayInstallmentAsync(Guid installmentId, PayLiabilityInstallmentRequest request)
        {
            var installment = await repository.GetInstallmentByIdAsync(installmentId);
            installment.Pay(request.PaidAt);
            await unitOfWork.CommitAsync();
            return LiabilityInstallmentResponse.Create(installment);
        }

        public async Task<LiabilityInstallmentResponse> UnpayInstallmentAsync(Guid installmentId)
        {
            var installment = await repository.GetInstallmentByIdAsync(installmentId);
            installment.Unpay();
            await unitOfWork.CommitAsync();
            return LiabilityInstallmentResponse.Create(installment);
        }
    }
}
