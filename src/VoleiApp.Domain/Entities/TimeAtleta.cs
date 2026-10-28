namespace VoleiApp.Domain.Entities
{
    public class TimeAtleta
    {
        public int ID { get; set; }

        public int TimeId { get; set; }
        public Time? Time { get; set; }

        public int AtletaId { get; set; }
        public Atleta? Atleta { get; set; }
    }
}
