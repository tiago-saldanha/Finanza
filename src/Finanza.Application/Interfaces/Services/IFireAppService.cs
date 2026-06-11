using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IFireAppService
    {
        Task<FireResponse> GetAsync();
    }
}
