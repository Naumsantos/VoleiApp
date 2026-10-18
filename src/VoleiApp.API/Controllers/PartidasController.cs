using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartidasController : ControllerBase
{
    private readonly IPartidaService _partidaService;

    public PartidasController(IPartidaService partidaService)
        => _partidaService = partidaService;

    [HttpPost]
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

    [HttpGet]
    public async Task<ActionResult<List<PartidaResponseDTO>>> GetAll(CancellationToken ct)
        => Ok(await _partidaService.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<PartidaResponseDTO>> GetById(int id, CancellationToken ct)
    {
        var partida = await _partidaService.GetByIdAsync(id, ct);
        return partida is null ? NotFound() : Ok(partida);
    }

    [HttpPost("{id}/resultado")]
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