using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using VoleiApp.Data;
using VoleiApp.Models;

namespace VoleiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeController : ControllerBase
    {
        private readonly VoleiContext _context;

        public TimeController(VoleiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os times cadastrados.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Time>>> GetAll()
        {
            return await _context.Times.Include(t => t.Atletas).ToListAsync();
        }

        /// <summary>
        /// Retorna um time por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Time>> GetById(int id)
        {
            var time = await _context.Times.Include(t => t.Atletas).FirstOrDefaultAsync(t => t.ID == id);

            return time == null ? NotFound() : Ok(time);
        }

        /// <summary>
        /// Cria um novo time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Time>> Create(Time time)
        {
            _context.Times.Add(time);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new {id = time.ID}, time);
        }

        /// <summary>
        /// Atualiza um time existente.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(int id, Time time)
        {
            if (id != time.ID) return BadRequest();

            _context.Entry(time).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var time = await _context.Times.FindAsync(id);
            if (time == null) return NotFound();

            _context.Times.Remove(time);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
