using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface INetWorthAppService
    {
        Task<NetWorthResponse> GetAsync();
    }
}
