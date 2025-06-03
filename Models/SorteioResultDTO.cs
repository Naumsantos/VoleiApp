using VoleiApp.Models;

/// <summary>
/// Resultado do sorteio com os times e reservas.
/// </summary>
public class SorteioResultDTO
{
    /// <summary>
    /// Times sorteados com seus atletas.
    /// </summary>
    public List<Time> Times { get; set; }

    /// <summary>
    /// Lista de atletas que ficaram como reservas.
    /// </summary>
    public List<Atleta> Reservas { get; set; }
}
