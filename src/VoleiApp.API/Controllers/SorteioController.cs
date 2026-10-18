using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SorteioController : ControllerBase
{
    private readonly ISorteioService _sorteio;

    public SorteioController(ISorteioService sorteio)
        => _sorteio = sorteio;

    [HttpPost("sortear")]
    public async Task<ActionResult<SorteioResultDTO>> Sortear([FromBody] SorteioConfigDTO config)
    {
        if (config.TamanhoDoTime != 4)
            return BadRequest("No MVP, o tamanho do time deve ser 4.");

        if (config.Atletas is null || !config.Atletas.Any())
            return BadRequest("Lista de atletas não pode estar vazia.");

        var resultado = await _sorteio.SortearTimes(config);
        return Ok(resultado);
    }
}