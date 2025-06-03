using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoleiApp.Data;
using VoleiApp.Models;

namespace VoleiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubstituicoesController : ControllerBase
    {
        readonly VoleiContext _context;

        public SubstituicoesController( VoleiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todas as substituições.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Substituicao>>> GetAll()
        {
            var subs = await _context.Substituicoes
                                    .Include(s => s.Entraram)
                                    .Include(s => s.Sairam)
                                    .ToListAsync();

            return Ok(subs);
        }

        /// <summary>
        /// Retorna uma substituição pelo ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult<Substituicao>> GetById(int id)
        {
            var substituicao = await _context.Substituicoes
                .Include(s => s.Entraram)
                .Include(s => s.Sairam)
                .FirstOrDefaultAsync(s => s.TimeID == id);

            if (substituicao == null) return NotFound();

            return substituicao;
        }

        /// <summary>
        /// Registra uma substituição em uma partida.
        /// </summary>
        /// <param name="substituicao"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Substituicao>> Create(Substituicao substituicao)
        {
            _context.Substituicoes.Add(substituicao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = substituicao.TimeID }, substituicao);
        }
    }
}
