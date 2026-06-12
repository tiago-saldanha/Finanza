using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class LoanAppService(ILoanReceivableRepository repository, IUnitOfWork unitOfWork) : ILoanAppService
    {
        public async Task<IEnumerable<LoanResponse>> GetAllAsync()
            => (await repository.GetAllAsync()).Select(LoanResponse.Create);

        public async Task<LoanResponse> GetByIdAsync(Guid id)
            => LoanResponse.Create(await repository.GetByIdAsync(id));

        public async Task<LoanSummaryResponse> GetSummaryAsync()
            => LoanSummaryResponse.Create(await repository.GetAllAsync());

        public async Task<LoanResponse> CreateAsync(LoanRequest request)
        {
            var loan = LoanReceivable.Create(
                request.BorrowerName,
                request.TotalAmount,
                request.StartDate,
                request.DueDate,
                request.Notes,
                request.InstallmentCount
            );
            await repository.AddAsync(loan);
            await unitOfWork.CommitAsync();
            return LoanResponse.Create(loan);
        }

        public async Task<LoanResponse> UpdateAsync(Guid id, LoanRequest request)
        {
            var loan = await repository.GetByIdAsync(id);
            loan.Update(request.BorrowerName, request.TotalAmount, request.StartDate, request.DueDate, request.Notes);
            await repository.UpdateAsync(loan);
            await unitOfWork.CommitAsync();
            return LoanResponse.Create(loan);
        }

        public async Task DeleteAsync(Guid id)
        {
            var loan = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(loan);
            await unitOfWork.CommitAsync();
        }

        public async Task<LoanInstallmentResponse> PayInstallmentAsync(Guid installmentId, PayInstallmentRequest request)
        {
            var installment = await repository.GetInstallmentByIdAsync(installmentId);
            installment.Pay(request.PaidAt);
            await unitOfWork.CommitAsync();
            return LoanInstallmentResponse.Create(installment);
        }

        public async Task<LoanInstallmentResponse> UnpayInstallmentAsync(Guid installmentId)
        {
            var installment = await repository.GetInstallmentByIdAsync(installmentId);
            installment.Unpay();
            await unitOfWork.CommitAsync();
            return LoanInstallmentResponse.Create(installment);
        }
    }
}
