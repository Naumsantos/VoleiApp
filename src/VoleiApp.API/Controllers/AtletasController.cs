using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Atleta;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtletasController : ControllerBase
    {
        private readonly IAtletaService _atletaService;

        public AtletasController(IAtletaService atletaService)
        {
            _atletaService = atletaService;
        }

        /// <summary>
        /// Retorna todos os atletas cadastrados.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AtletaResponseDTO>>> GetAll(CancellationToken ct)
        {
            var atletas = await _atletaService.GetAllAsync(ct);
            return Ok(atletas);
        }

        /// <summary>
        /// Retorna um atleta por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AtletaResponseDTO>> GetById(int ID, CancellationToken ct)
        {
            var atleta = await _atletaService.GetByIdAsync(ID, ct);

            return atleta == null ? NotFound() : Ok(atleta);
        }

        /// <summary>
        /// Cadastra um novo atleta
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AtletaResponseDTO>> Create([FromBody] CriarAtletaDTO atleta, CancellationToken ct)
        {
            try
            {
                var atletaCriado = await _atletaService.CreateAsync(atleta, ct);

                return CreatedAtAction(nameof(GetById), new { id = atletaCriado.ID }, atletaCriado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza os dados do atleta existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AtualizarAtletaDTO atleta, CancellationToken ct)
        {
            try
            {
                await _atletaService.UpdateAsync(id, atleta, ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deleta um atleta por ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _atletaService.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
