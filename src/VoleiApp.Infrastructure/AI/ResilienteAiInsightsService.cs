using Microsoft.Extensions.Logging;
using VoleiApp.Application.DTOs.Insights;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.Infrastructure.AI
{
    public class ResilienteAiInsightsService : IAiInsightsService
    {
        private readonly OllamaAiInsightsService _primary;
        private readonly FallbackInsightsService _fallback;
        private readonly ILogger<ResilienteAiInsightsService> _logger;

        public ResilienteAiInsightsService(
            OllamaAiInsightsService primary,
            FallbackInsightsService fallback,
            ILogger<ResilienteAiInsightsService> logger)
        {
            _primary = primary;
            _fallback = fallback;
            _logger = logger;
        }

        public async Task<InsightPartidaResponseDTO> GerarInsightPartidaAsync(
            PartidaResponseDTO partida,
            GerarInsightPartidaRequestDTO? request,
            CancellationToken ct = default)
        {
            try
            {
                return await _primary.GerarInsightPartidaAsync(partida, request, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha no provedor IA. Aplicando fallback local.");
                return await _fallback.GerarInsightPartidaAsync(partida, request, ct);
            }
        }
    }
}
