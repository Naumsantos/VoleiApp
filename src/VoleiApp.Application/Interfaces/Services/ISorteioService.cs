using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Domain.Entities;

namespace VoleiApp.Application.Interfaces.Services
{
    public interface ISorteioService
    {
        Task<SorteioResultDTO> SortearTimes(SorteioConfigDTO config);
        Task<Substituicao> SubstituirJogadores(Time timePerdedor, Queue<Atleta> reservas);
    }
}