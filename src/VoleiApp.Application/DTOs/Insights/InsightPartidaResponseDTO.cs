namespace VoleiApp.Application.DTOs.Insights
{
    public record InsightPartidaResponseDTO
    (
        string Resumo,
        List<string> Sugestoes,
        string Modelo,
        int? TokensEstimados,
        bool FallbackUsado
    );
}
