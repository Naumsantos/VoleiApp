namespace VoleiApp.Application.DTOs.Partida;

public class CriarPartidaDTO
{
    public TimeDTO TimeA { get; set; } = new();
    public TimeDTO TimeB { get; set; } = new();
    public List<int> ReservaIds { get; set; } = new();
}