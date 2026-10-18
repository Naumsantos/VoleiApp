namespace VoleiApp.Application.DTOs.Partida;

public class TimeResponseDTO
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<AtletaResumoDTO> Atletas { get; set; } = new();
}