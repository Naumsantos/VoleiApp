using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers
{
    /// <summary>
    /// Endpoints para consulta de substituições registradas nas partidas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubstituicoesController : ControllerBase
    {
        private readonly IPartidaService _partidaService;

        public SubstituicoesController(IPartidaService partidaService)
        {
            _partidaService = partidaService;
        }

        /// <summary>
        /// Lista todas as substituições cadastradas.
        /// </summary>
        /// <param name="ct">Token para cancelamento da operação.</param>
        /// <returns>Coleção de substituições.</returns>
        /// <response code="200">Substituições retornadas com sucesso.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubstituicaoResponseDTO>>> GetAll(CancellationToken ct)
        {
            var substituicoes = await _partidaService.GetSubstituicoesAsync(ct);
            return Ok(substituicoes);
        }

        /// <summary>
        /// Obtém uma substituição pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da substituição.</param>
        /// <param name="ct">Token para cancelamento da operação.</param>
        /// <returns>Dados da substituição encontrada.</returns>
        /// <response code="200">Substituição encontrada e retornada.</response>
        /// <response code="404">Substituição não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubstituicaoResponseDTO>> GetById(int id, CancellationToken ct)
        {
            var substituicao = await _partidaService.GetSubstituicaoByIdAsync(id, ct);
            return substituicao is null ? NotFound() : Ok(substituicao);
        }
    }
}
