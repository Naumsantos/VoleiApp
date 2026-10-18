namespace VoleiApp.Domain.Entities
{
    public class PartidaReserva
    {
        public int Id { get; set; }

        public int PartidaId { get; set; }
        public Partida Partida { get; set; } = new();

        public int AtletaId { get; set; }
        public Atleta Atleta { get; set; } = new();

        public int Ordem { get; set; }
    }
}
