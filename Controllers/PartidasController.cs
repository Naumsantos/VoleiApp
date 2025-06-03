using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoleiApp.Data;
using VoleiApp.Models;

namespace VoleiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartidasController : ControllerBase
    {
        readonly VoleiContext _context;

        public PartidasController(VoleiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ragistra a partida no banco
        /// </summary>
        /// <param name="partida"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegistrarPartida([FromBody] Partida partida)
        {
            _context.Partidas.Add(partida);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetByID), new { id = partida.ID }, partida);
        }

        /// <summary>
        /// Lista todas as partidas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Partida>> ListarPartidas()
        { 
            return _context.Partidas
                .Include(p => p.TimeA.Atletas)
                .Include(p => p.TimeB.Atletas)
                .Include(p => p.Substituicoes)
                .ToList();
        }

        /// <summary>
        /// Retorna partida pro id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Partida> GetByID(int id)
        { 
            var partida = _context.Partidas
                .Include(p => p.TimeA.Atletas)
                .Include(p => p.TimeB.Atletas)
                .Include(p => p.Substituicoes)
                .FirstOrDefault(p => p.ID == id);

            return partida == null ? NotFound() : Ok(partida);
        }

    }
}
