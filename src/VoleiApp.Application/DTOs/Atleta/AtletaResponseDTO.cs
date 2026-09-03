using VoleiApp.Domain.Enums;

namespace VoleiApp.Application.DTOs.Atleta
{
    public class AtletaResponseDTO
    {
        public int ID { get; set; }
        public string Nome { get; set; } = string.Empty;
        public EPosicao Posicao { get; set; }
    }
}
