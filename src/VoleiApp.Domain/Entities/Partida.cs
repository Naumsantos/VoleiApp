namespace VoleiApp.Domain.Entities
{
    public class Partida
    {
        public int ID { get; set; }

        public Time TimeA { get; set; } = new();
        public int TimeAId { get; set; }

        public Time TimeB { get; set; } = new();
        public int TimeBId { get; set; } = new();

        public int TimeVencedorId { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;
        public bool Encerrada { get; set; }

        public List<PartidaReserva> Reservas { get; set; } = new();
        public List<Substituicao> Substituicoes { get; set; } = new();
    }
}
