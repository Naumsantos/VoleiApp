using VoleiApp.Domain.Entities;

namespace VoleiApp.Application.DTOs.Partida
{
    public record CriarTimeDTO(
                string Nome,
                List<TimeAtleta> Atletas
        );
}
