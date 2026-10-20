namespace VoleiApp.Application.DTOs.Partida
{
    public class SalvarPartidaDTO
    {
        public SalvarTimeDTO TimeA { get; set; } = new();
        public SalvarTimeDTO TimeB { get; set; } = new();
        public List<int> ReservaIds { get; set; } = new();
    }
}
