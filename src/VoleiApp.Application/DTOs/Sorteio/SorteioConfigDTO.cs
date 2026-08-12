using VoleiApp.Domain.Entities;

namespace VoleiApp.Application.DTOs.Sorteio;

/// <summary>
/// Representa as configurações para sorteio de times.
/// </summary>
public class SorteioConfigDTO
{
    /// <summary>
    /// Lista de atletas disponíveis para o sorteio.
    /// </summary>
    public List<Atleta> Atletas { get; set; } = new();

    /// <summary>
    /// Quantidade de atletas por time (2, 4 ou 6).
    /// </summary>
    public int TamanhoDoTime { get; set; } = 4;

    /// <summary>
    /// Quantidade de atacantes (Ponteiro/Oposto) por time.
    /// </summary>
    public int AtacantesPorTime { get; set; } = 2;

    /// <summary>
    /// Quantidade de levantadores por time.
    /// </summary>
    public int LevantadoresPorTime { get; set; } = 1;

    /// <summary>
    /// Quantidade de meios (Central/Libero) por time.
    /// </summary>
    public int MeiosPorTime { get; set; } = 1;

    /// <summary>
    /// Permite completar times com fallback quando faltar posição.
    /// </summary>
    public bool PermitirFallback { get; set; } = true;
}