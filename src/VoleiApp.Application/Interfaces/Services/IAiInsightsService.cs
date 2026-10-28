using VoleiApp.Application.DTOs.Insights;
using VoleiApp.Application.DTOs.Partida;

namespace VoleiApp.Application.Interfaces.Services
{
    public interface IAiInsightsService
    {
        Task<InsightPartidaResponseDTO> GerarInsightPartidaAsync(
                PartidaResponseDTO partida,
                GerarInsightPartidaRequestDTO? request,
                CancellationToken ct = default
            );
    }
}
