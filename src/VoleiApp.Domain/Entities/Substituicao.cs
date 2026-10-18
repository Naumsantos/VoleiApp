namespace VoleiApp.Domain.Entities
{
    public class Substituicao
    {
        public int Id { get; set; }

        public int PartidaId { get; set; }
        public Partida Partida { get; set; } = new();

        public int TimeId { get; set; }
        public Time Time { get; set; } = new();

        public DateTime Data { get; set; } = DateTime.UtcNow;

        public List<Atleta> Entraram { get; set; } = new List<Atleta>();
        public List<Atleta> Sairam { get; set; } = new List<Atleta>();
    }
}
