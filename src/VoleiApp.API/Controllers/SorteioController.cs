using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers;

/// <summary>
/// Execução dos sorteios
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SorteioController : ControllerBase
{
    private readonly ISorteioService _sorteio;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    /// <param name="sorteio"></param>
    public SorteioController(ISorteioService sorteio)
        => _sorteio = sorteio;

    /// <summary>
    /// Realiza o sorteio dos times
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    [HttpPost("sortear")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SorteioResultDTO>> Sortear([FromBody] SorteioConfigDTO config)
    {
        var resultado = await _sorteio.SortearTimes(config);
        return Ok(resultado);
    }
}