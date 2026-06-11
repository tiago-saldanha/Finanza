using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Application.Exceptions;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;

namespace Finanza.Application.Services
{
    public class FinancialAccountAppService(IFinancialAccountRepository repository, IUnitOfWork unitOfWork) : IFinancialAccountAppService
    {
        public async Task<IEnumerable<FinancialAccountResponse>> GetAllAsync()
        {
            var accounts = await repository.GetAllAsync();
            return accounts.Select(FinancialAccountResponse.Create);
        }

        public async Task<FinancialAccountResponse> GetByIdAsync(Guid id)
        {
            var account = await repository.GetByIdAsync(id);
            return FinancialAccountResponse.Create(account);
        }

        public async Task<FinancialAccountResponse> CreateAsync(FinancialAccountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new AccountNameAppException();
            var account = Account.Create(request.Name, request.Type, request.InitialBalance);
            await repository.AddAsync(account);
            await unitOfWork.CommitAsync();
            return FinancialAccountResponse.Create(account);
        }

        public async Task<FinancialAccountResponse> UpdateAsync(Guid id, FinancialAccountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new AccountNameAppException();
            var account = await repository.GetByIdAsync(id);
            account.Update(request.Name, request.Type, request.InitialBalance);
            await repository.UpdateAsync(account);
            await unitOfWork.CommitAsync();
            return FinancialAccountResponse.Create(account);
        }

        public async Task DeleteAsync(Guid id)
        {
            var account = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(account);
            await unitOfWork.CommitAsync();
        }
    }
}
