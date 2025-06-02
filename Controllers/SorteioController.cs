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
        /// Realiza o sorteio dos times com base na configuração.
        /// </summary>
        [HttpPost("sortear")]
        public IActionResult Sortear([FromBody] SorteioConfig config)
        {
            var atletas = _context.Atletas.ToList();
            var totalNecessario = config.NumeroDeTimes * config.JogadorPorTimes;

            if(atletas.Count < totalNecessario) 
                return BadRequest($"É necessário pelo menos {totalNecessario} atletas cadastrados.");

            _times = _sorteioService.SortearTimes(atletas, config, out _reservas);
            return Ok( _times );
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
    }
}
