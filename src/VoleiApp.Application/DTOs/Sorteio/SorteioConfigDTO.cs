using VoleiApp.Models;

/// <summary>
/// Representa as configurações para sorteio de times.
/// </summary>
public class SorteioConfigDTO
{
    /// <summary>
    /// Lista de atletas disponíveis para o sorteio.
    /// </summary>
    public List<Atleta> Atletas { get; set; }

    /// <summary>
    /// Quantidade de atletas por time (2, 4 ou 6).
    /// </summary>
    public int TamanhoDoTime { get; set; } = 4;
}
