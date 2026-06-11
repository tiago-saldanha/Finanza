using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface ILiabilityAppService
    {
        Task<IEnumerable<LiabilityResponse>> GetAllAsync();
        Task<LiabilityResponse>              GetByIdAsync(Guid id);
        Task<LiabilityResponse>              CreateAsync(LiabilityRequest request);
        Task<LiabilityResponse>              UpdateAsync(Guid id, LiabilityRequest request);
        Task                                 DeleteAsync(Guid id);
    }
}
