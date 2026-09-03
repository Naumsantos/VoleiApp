using VoleiApp.Domain.Entities;

namespace VoleiApp.Application.DTOs.Partida
{
    /// <summary>
    /// Representa uma requisição para salvar uma partida gerada pelo sorteio.
    /// </summary>
    public class SalvarPartidaDTO
    {
        public Time TimeA { get; set; } = new();
        public Time TimeB { get; set; } = new();
        public List<VoleiApp.Domain.Entities.Atleta>? Reservas { get; set; }
    }
}
