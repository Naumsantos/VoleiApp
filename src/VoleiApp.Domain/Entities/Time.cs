namespace VoleiApp.Domain.Entities
{
    public class Time
    {
        public int ID { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<TimeAtleta> Atletas { get; set; } = new();

        public int? PartidaId { get; set; }
        public Partida? Partida { get; set; }
    }
}
