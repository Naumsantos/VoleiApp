namespace VoleiApp.Models
{
    public class Partida
    {
        public int ID { get; set; }
        public Time TimeA { get; set; }
        public Time TimeB { get; set; }
        public int TimeVencedorID {  get; set; }
        public DateTime Data {  get; set; } = DateTime.Now;
        public List<Substituicao> Substuicoes { get; set; }
    }
}
