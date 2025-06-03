using Microsoft.AspNetCore.Mvc;
using VoleiApp.Data;
using VoleiApp.Models;
using VoleiApp.Services;

namespace VoleiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SorteioController : ControllerBase
    {
        private readonly VoleiContext _context;
        private readonly SorteioService _sorteioService;
        private static List<Time> _times = new();
        private static Queue<Atleta> _reservas = new();

        public SorteioController(VoleiContext context)
        {
            _context = context;
            _sorteioService = new SorteioService();
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

            var resultado = _sorteioService.SortearTimes(config);
            return Ok(resultado);
        }

        /// <summary>
        /// Substitui jogadores do time perdedor com jogadores da fila de reservas.
        /// </summary>
        [HttpPost("substituir/{idDoTime}")]
        public IActionResult Subistituir(int idDoTime)
        {
            var time = _times.FirstOrDefault(time => time.ID == idDoTime);
            if(time == null)
            {
                return NotFound("Time não encontrato");
            }

            var substituicao = _sorteioService.SubstituirJogadores(time, _reservas);
            return Ok( substituicao );
        }

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

            _context.Partidas.Add(partida);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(SalvarPartida), new { id = partida.Id }, partida);
        }
    }
}
