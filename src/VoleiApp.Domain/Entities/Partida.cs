namespace VoleiApp.Domain.Entities
{
    public class Partida
    {
        public int ID { get; set; }
        public Time TimeA { get; set; } = new();
        public Time TimeB { get; set; } = new();
        public int TimeVencedorID { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public List<Substituicao> Substituicoes { get; set; } = new();
    }
}
