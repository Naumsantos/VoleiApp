namespace VoleiApp.Application.DTOs.Partida;

public class PartidaResponseDTO
{
    public int ID { get; set; }
    public DateTime Data { get; set; }
    public bool Encerrada { get; set; }
    public int? TimeVencedorId { get; set; }

    public TimeResponseDTO TimeA { get; set; } = new();
    public TimeResponseDTO TimeB { get; set; } = new();

    public List<AtletaResumoDTO> Reservas { get; set; } = new();
    public List<SubstituicaoResponseDTO> Substituicoes { get; set; } = new();
}