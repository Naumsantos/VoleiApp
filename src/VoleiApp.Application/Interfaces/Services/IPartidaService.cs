using VoleiApp.Application.DTOs.Partida;

namespace VoleiApp.Application.Interfaces.Services;

public interface IPartidaService
{
    Task<PartidaResponseDTO> CreateAsync(CriarPartidaDTO dto, CancellationToken ct = default);
    Task<PartidaResponseDTO> SalvarPartidaAsync(SalvarPartidaDTO dto, CancellationToken ct = default);
    Task<List<PartidaResponseDTO>> GetAllAsync(CancellationToken ct = default);
    Task<PartidaResponseDTO?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<SubstituicaoResponseDTO>> GetSubstituicoesAsync(CancellationToken ct = default);
    Task<SubstituicaoResponseDTO?> GetSubstituicaoByIdAsync(int id, CancellationToken ct = default);
    Task<PartidaResponseDTO> RegistrarResultadoAsync(int partidaId, RegistrarResultadoPartidaDTO dto, CancellationToken ct = default);
}
