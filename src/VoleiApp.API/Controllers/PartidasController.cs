using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers;

/// <summary>
/// Gerenciamento de partidas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Gerenciamento de partidas")]
public class PartidasController : ControllerBase
{
    private readonly IPartidaService _partidaService;

    public PartidasController(IPartidaService partidaService)
        => _partidaService = partidaService;

    /// <summary>
    /// Cria uma nova partida.
    /// </summary>
    /// <param name="dto">Dados para criação da partida.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Partida criada.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Cria uma partida", Description = "Cria uma nova partida com os dados fornecidos.")]
    [SwaggerResponse(StatusCodes.Status201Created, type: typeof(PartidaResponseDTO), description: "Partida criada com sucesso.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, description: "Dados inválidos.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, description: "Recurso relacionado não encontrado.")]
    public async Task<ActionResult<PartidaResponseDTO>> Create([FromBody] CriarPartidaDTO dto, CancellationToken ct)
    {
        try
        {
            var partida = await _partidaService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = partida.ID }, partida);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Salva uma partida (fluxo de salvar progresso).
    /// </summary>
    /// <param name="dto">Dados para salvar a partida.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Partida salva.</returns>
    [HttpPost("salvar")]
    [SwaggerOperation(Summary = "Salvar partida", Description = "Salva o estado de uma partida (salvar progresso).")]
    [SwaggerResponse(StatusCodes.Status201Created, type: typeof(PartidaResponseDTO), description: "Partida salva com sucesso.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, description: "Dados inválidos.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, description: "Recurso relacionado não encontrado.")]
    public async Task<ActionResult<PartidaResponseDTO>> SalvarPartida([FromBody] SalvarPartidaDTO dto, CancellationToken ct)
    {
        try
        {
            var partida = await _partidaService.SalvarPartidaAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = partida.ID }, partida);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Retorna todas as partidas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de partidas.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar partidas", Description = "Retorna todas as partidas.")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<PartidaResponseDTO>), description: "Lista de partidas.")]
    public async Task<ActionResult<List<PartidaResponseDTO>>> GetAll(CancellationToken ct)
        => Ok(await _partidaService.GetAllAsync(ct));

    /// <summary>
    /// Retorna uma partida por ID.
    /// </summary>
    /// <param name="id">ID da partida.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Partida encontrada ou 404.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter partida por ID", Description = "Retorna os detalhes de uma partida informando o ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PartidaResponseDTO), description: "Partida encontrada.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, description: "Partida não encontrada.")]
    public async Task<ActionResult<PartidaResponseDTO>> GetById(int id, CancellationToken ct)
    {
        var partida = await _partidaService.GetByIdAsync(id, ct);
        return partida is null ? NotFound() : Ok(partida);
    }

    /// <summary>
    /// Registra o resultado de uma partida.
    /// </summary>
    /// <param name="id">ID da partida.</param>
    /// <param name="dto">Resultado a ser registrado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Partida atualizada com o resultado.</returns>
    [HttpPost("{id}/resultado")]
    [SwaggerOperation(Summary = "Registrar resultado", Description = "Registra o resultado final de uma partida.")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PartidaResponseDTO), description: "Resultado registrado com sucesso.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, description: "Dados inválidos.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, description: "Partida ou recurso relacionado não encontrado.")]
    [SwaggerResponse(StatusCodes.Status409Conflict, description: "Conflito ao registrar o resultado (estado inválido).")]
    public async Task<ActionResult<PartidaResponseDTO>> RegistrarResultado(int id, [FromBody] RegistrarResultadoPartidaDTO dto, CancellationToken ct)
    {
        try
        {
            var partida = await _partidaService.RegistrarResultadoAsync(id, dto, ct);
            return Ok(partida);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}