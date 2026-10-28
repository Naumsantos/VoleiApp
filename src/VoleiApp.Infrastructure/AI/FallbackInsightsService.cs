using VoleiApp.Application.DTOs.Insights;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.Infrastructure.AI
{
    public class FallbackInsightsService : IAiInsightsService
    {
        public Task<InsightPartidaResponseDTO> GerarInsightPartidaAsync(PartidaResponseDTO partida, GerarInsightPartidaRequestDTO? request, CancellationToken ct = default)
        {
            var resumo = $"Partida {partida.ID}: Time A ({partida.TimeA.Atletas.Count}) x Time B ({partida.TimeB.Atletas.Count})," +
                         $"Reservas: {partida.Reservas.Count}, Substituições: {partida.Substituicoes.Count}";

            var sugestoes = new List<string>
            {
                "Monitorar equilíbrio por posição nas próximas partidas.",
                "Avaliar rotação de reservas para reduzir desgaste dos titulares.",
                "Registrar notas de desempenho para evoluir o balanceamento."
            };

            return Task.FromResult(new InsightPartidaResponseDTO(
                resumo,
                sugestoes,
                "fallback-local",
                null,
                true
            ));
        }
    }
}
