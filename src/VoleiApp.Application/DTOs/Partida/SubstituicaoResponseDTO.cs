namespace VoleiApp.Application.DTOs.Partida;

public class SubstituicaoResponseDTO
{
    public int Id { get; set; }
    public int TimeId { get; set; }
    public DateTime Data { get; set; }
    public List<AtletaResumoDTO> Entraram { get; set; } = new();
    public List<AtletaResumoDTO> Sairam { get; set; } = new();
}