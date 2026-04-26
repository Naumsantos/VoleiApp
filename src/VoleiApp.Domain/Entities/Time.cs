namespace VoleiApp.Domain.Entities
{
    public class Time
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public  List<Atleta> Atletas { get; set; }
    }
}
