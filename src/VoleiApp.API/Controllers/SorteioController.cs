using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.DTOs.Sorteio;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Interfaces.Repositories;

namespace VoleiApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SorteioController : ControllerBase
    {
        readonly ISorteioService _sorteio;
        readonly IPartidaRepository _partida;

        public SorteioController(ISorteioService sorteio, IPartidaRepository partida)
        {
            _sorteio = sorteio;
            _partida = partida;
        }

        /// <summary>
        /// Realiza o sorteio automático de times a partir da lista de atletas.
        /// </summary>
        /// <param name="config">Configuração do sorteio (atletas + tamanho do time)</param>
        [HttpPost("sortear")]
        public ActionResult<SorteioResultDTO> Sortear(SorteioConfigDTO config)
        {
            if (config.TamanhoDoTime != 2 && config.TamanhoDoTime != 4 && config.TamanhoDoTime != 6)
                return BadRequest("Tamanho do time deve ser 2, 4 ou 6.");

            if (config.Atletas == null || !config.Atletas.Any())
                return BadRequest("Lista de atletas não pode estar vazia.");

            var resultado = _sorteio.SortearTimes(config);
            return Ok(resultado);
        }

        /// <summary>
        /// Substitui jogadores do time perdedor com jogadores da fila de reservas.
        /// </summary>
        //[HttpPost("substituir/{idDoTime}")]
        //public IActionResult Subistituir(int idDoTime)
        //{
        //    var time = _times.FirstOrDefault(time => time.ID == idDoTime);
        //    if (time == null)
        //    {
        //        return NotFound("Time não encontrato");
        //    }

        //    var substituicao = _sorteioService.SubstituirJogadores(time, _reservas);
        //    return Ok(substituicao);
        //}

        /// <summary>
        /// Salva uma nova partida a partir do resultado do sorteio.
        /// </summary>
        [HttpPost("salvar")]
        public async Task<ActionResult<Partida>> SalvarPartida(SalvarPartidaDTO dto)
        {
            if (dto.TimeA == null || dto.TimeB == null)
                return BadRequest("Time A e Time B são obrigatórios.");

            var partida = new Partida
            {
                TimeA = dto.TimeA,
                TimeB = dto.TimeB,
                Substituicoes = new List<Substituicao>()
            };

            await _partida.AddAsync(partida);

            return CreatedAtAction(nameof(SalvarPartida), new { id = partida.ID }, partida);
        }
    }
}
