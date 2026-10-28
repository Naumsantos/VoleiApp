using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Domain.Entities;

namespace VoleiApp.Application.Interfaces.Services
{
    public interface ITimeService
    {
        Task<IEnumerable<TimeCompletoResponseDTO>> GetAllAsync(CancellationToken ct);
        Task<Time> GetByIdAsync(int id, CancellationToken ct);
        Task<TimeResponseDTO> CreateAsync(CriarTimeDTO time, CancellationToken ct);
        Task UpdateAsync(int id, TimeDTO time, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
