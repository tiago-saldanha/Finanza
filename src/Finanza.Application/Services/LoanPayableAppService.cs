using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class LoanPayableAppService(ILoanPayableRepository repository, IUnitOfWork unitOfWork) : ILoanPayableAppService
    {
        public async Task<IEnumerable<LoanPayableResponse>> GetAllAsync()
            => (await repository.GetAllAsync()).Select(LoanPayableResponse.Create);

        public async Task<LoanPayableResponse> GetByIdAsync(Guid id)
            => LoanPayableResponse.Create(await repository.GetByIdAsync(id));

        public async Task<LoanPayableSummaryResponse> GetSummaryAsync()
            => LoanPayableSummaryResponse.Create(await repository.GetAllAsync());

        public async Task<LoanPayableResponse> CreateAsync(LoanPayableRequest request)
        {
            var loan = LoanPayable.Create(
                request.CreditorName,
                request.TotalAmount,
                request.StartDate,
                request.DueDate,
                request.Notes,
                request.InstallmentCount
            );
            await repository.AddAsync(loan);
            await unitOfWork.CommitAsync();
            return LoanPayableResponse.Create(loan);
        }

        public async Task<LoanPayableResponse> UpdateAsync(Guid id, LoanPayableRequest request)
        {
            var loan = await repository.GetByIdAsync(id);
            loan.Update(request.CreditorName, request.TotalAmount, request.StartDate, request.DueDate, request.Notes);
            await repository.UpdateAsync(loan);
            await unitOfWork.CommitAsync();
            return LoanPayableResponse.Create(loan);
        }

        public async Task DeleteAsync(Guid id)
        {
            var loan = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(loan);
            await unitOfWork.CommitAsync();
        }

        public async Task<LoanPayableInstallmentResponse> PayInstallmentAsync(Guid installmentId, PayPayableInstallmentRequest request)
        {
            var installment = await repository.GetInstallmentByIdAsync(installmentId);
            installment.Pay(request.PaidAt);
            await unitOfWork.CommitAsync();
            return LoanPayableInstallmentResponse.Create(installment);
        }

        public async Task<LoanPayableInstallmentResponse> UnpayInstallmentAsync(Guid installmentId)
        {
            var installment = await repository.GetInstallmentByIdAsync(installmentId);
            installment.Unpay();
            await unitOfWork.CommitAsync();
            return LoanPayableInstallmentResponse.Create(installment);
        }
    }
}
