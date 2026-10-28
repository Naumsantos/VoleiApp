using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VoleiApp.Application.DTOs.Insights;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Insights com IA para partidas")]
public class InsightsController : ControllerBase
{
    private readonly IPartidaService _partidaService;
    private readonly IAiInsightsService _aiInsightsService;

    public InsightsController(IPartidaService partidaService, IAiInsightsService aiInsightsService)
    {
        _partidaService = partidaService;
        _aiInsightsService = aiInsightsService;
    }

    [HttpPost("partidas/{id:int}")]
    [SwaggerOperation(
        Summary = "Gera insight de uma partida",
        Description = "Gera resumo e sugestões da partida com IA, com fallback local.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Insight gerado.", typeof(InsightPartidaResponseDTO))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Partida não encontrada.")]
    public async Task<ActionResult<InsightPartidaResponseDTO>> GerarInsightPartida(
        int id,
        [FromBody] GerarInsightPartidaRequestDTO? request,
        CancellationToken ct)
    {
        var partida = await _partidaService.GetByIdAsync(id, ct);
        if (partida is null)
            return NotFound($"Partida {id} não encontrada.");

        var response = await _aiInsightsService.GerarInsightPartidaAsync(partida, request, ct);
        return Ok(response);
    }
}