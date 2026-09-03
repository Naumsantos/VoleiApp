using VoleiApp.Application.DTOs.Atleta;

namespace VoleiApp.Application.Interfaces.Services
{
    public interface IAtletaService
    {
        Task<List<AtletaResponseDTO>> GetAllAsync(CancellationToken ct = default);
        Task<AtletaResponseDTO?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AtletaResponseDTO> CreateAsync(CriarAtletaDTO dto, CancellationToken ct = default);
        Task UpdateAsync(int id, AtualizarAtletaDTO dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
