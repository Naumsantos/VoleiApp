using VoleiApp.Application.DTOs.Partida;

namespace VoleiApp.Application.Interfaces.Services
{
    public interface ITimeService
    {
        Task<IEnumerable<TimeDTO>> GetAllAsync(CancellationToken ct);
        Task<TimeDTO> GetByIdAsync(int id, CancellationToken ct);
        Task<TimeResponseDTO> CreateAsync(CriarTimeDTO time, CancellationToken ct);
        Task UpdateAsync(int id, TimeDTO time, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
