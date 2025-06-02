using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoleiApp.Data;
using VoleiApp.Models;

namespace VoleiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtletasController : ControllerBase
    {
        private readonly VoleiContext _context;

        public AtletasController( VoleiContext context )
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os atletas cadastrados.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Atleta>> GetAll()
        {
           return _context.Atletas.ToList();
        }

        /// <summary>
        /// Retorna um atleta por ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Atleta> GetById(int ID)
        {
            var atleta = _context.Atletas.Find(ID);

            return atleta == null ? NotFound() : atleta;
        }

        /// <summary>
        /// Cadastra um novo atleta
        /// </summary>
        [HttpPost]
        public ActionResult<Atleta> Create([FromBody] Atleta atleta)
        {
            _context.Atletas.Add(atleta);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new {id = atleta.ID}, atleta);
        }

        /// <summary>
        /// Atualiza os dados do atleta existente.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Atleta atleta)
        {
            if (id != atleta.ID) return BadRequest();

            if (!_context.Atletas.Any( a => a.ID == id)) return NotFound();

            _context.Entry(atleta).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deleta um atleta por ID
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var atleta = _context.Atletas.Find(id);
            if (atleta == null) return NotFound();

            _context.Atletas.Remove(atleta);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
