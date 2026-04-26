namespace VoleiApp.Domain.Entities
{
    public class Substituicao
    {
        public int Id { get; set; }
        public List<Atleta> Entraram { get; set; } = new List<Atleta>();
        public List<Atleta> Sairam { get; set; } = new List<Atleta>();
    }
}
