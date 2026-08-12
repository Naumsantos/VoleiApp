using VoleiApp.Domain.Enums;

namespace VoleiApp.Domain.Entities
{
    public class Atleta
    {
        public int ID { get; set; }
        public string Nome { get; set; } = string.Empty;
        public EPosicao Posicao { get; set; }
    }
}
