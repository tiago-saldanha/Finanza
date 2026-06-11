using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface IPatrimonySnapshotAppService
    {
        Task<IEnumerable<PatrimonySnapshotResponse>> GetAllAsync();
        Task<PatrimonySnapshotResponse>              CreateAsync();
    }
}
