namespace VoleiApp.Application.DTOs.Partida;

public class TimeDTO
{
    public string Nome { get; set; } = string.Empty;
    public List<int> AtletaIds { get; set; } = new();
}