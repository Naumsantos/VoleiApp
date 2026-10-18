using VoleiApp.Domain.Enums;

namespace VoleiApp.Application.DTOs.Partida;

public class AtletaResumoDTO
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public EPosicao Posicao { get; set; }
}