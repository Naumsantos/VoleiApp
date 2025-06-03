namespace VoleiApp.Models
{
    /// <summary>
    /// Representa uma requisição para salvar uma partida gerada pelo sorteio.
    /// </summary>
    public class SalvarPartidaDTO
    {
        public Time TimeA { get; set; }
        public Time TimeB { get; set; }
        public List<Atleta>? Reservas { get; set; }
    }
}
