using VoleiApp.Domain.Entities;

namespace VoleiApp.Application.DTOs.Sorteio;

/// <summary>
/// Resultado do sorteio com os times e reservas.
/// </summary>
public class SorteioResultDTO
{
    /// <summary>
    /// Times sorteados com seus atletas.
    /// </summary>
    public List<Time> Times { get; set; } = new();

    /// <summary>
    /// Lista de atletas que ficaram como reservas.
    /// </summary>
    public List<Atleta> Reservas { get; set; } = new();

    /// <summary>
    /// Avisos sobre fallback ou times incompletos.
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Indica se algum time usou fallback de posição.
    /// </summary>
    public bool FallbackAplicado { get; set; }

    /// <summary>
    /// Quantidade de times que não ficaram no formato ideal.
    /// </summary>
    public int TimesIncompletos { get; set; }
}
