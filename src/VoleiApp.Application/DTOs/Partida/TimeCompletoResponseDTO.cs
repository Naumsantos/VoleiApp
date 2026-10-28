using VoleiApp.Application.DTOs.Atleta;

namespace VoleiApp.Application.DTOs.Partida;

public class TimeCompletoResponseDTO
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<TimeAtletaResponseDTO> Atletas { get; set; } = new();
}

public class TimeAtletaResponseDTO
{
    public int ID { get; set; }
    public int TimeId { get; set; }
    public string Time { get; set; } = string.Empty;
    public int AtletaId { get; set; }
    public AtletaResponseDTO Atleta { get; set; } = new();
}
