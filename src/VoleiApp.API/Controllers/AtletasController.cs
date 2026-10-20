using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Atleta;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.API.Controllers
{
    /// <summary>
    /// Endpoints para gerenciamento de atletas.
    /// </summary>
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
        /// <param name="ct">Token para cancelamento da requisição.</param>
        /// <returns>Lista de atletas.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        [HttpGet]
        public async Task<ActionResult<List<AtletaResponseDTO>>> GetAll(CancellationToken ct)
        {
            var atletas = await _atletaService.GetAllAsync(ct);
            return Ok(atletas);
        }

        /// <summary>
        /// Retorna um atleta pelo identificador.
        /// </summary>
        /// <param name="ID">Identificador único do atleta.</param>
        /// <param name="ct">Token para cancelamento da requisição.</param>
        /// <returns>Atleta encontrado.</returns>
        /// <response code="200">Atleta retornado com sucesso.</response>
        /// <response code="404">Atleta não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<AtletaResponseDTO>> GetById(int ID, CancellationToken ct)
        {
            var atleta = await _atletaService.GetByIdAsync(ID, ct);

            return atleta == null ? NotFound() : Ok(atleta);
        }

        /// <summary>
        /// Cadastra um novo atleta.
        /// </summary>
        /// <param name="atleta">Dados do atleta a ser criado.</param>
        /// <param name="ct">Token para cancelamento da requisição.</param>
        /// <returns>Atleta criado.</returns>
        /// <response code="201">Atleta criado com sucesso.</response>
        /// <response code="400">Dados inválidos para criação.</response>
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
        /// Atualiza os dados de um atleta existente.
        /// </summary>
        /// <param name="id">Identificador do atleta.</param>
        /// <param name="atleta">Novos dados do atleta.</param>
        /// <param name="ct">Token para cancelamento da requisição.</param>
        /// <response code="204">Atleta atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos para atualização.</response>
        /// <response code="404">Atleta não encontrado.</response>
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
        /// Remove um atleta pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do atleta.</param>
        /// <param name="ct">Token para cancelamento da requisição.</param>
        /// <response code="204">Atleta removido com sucesso.</response>
        /// <response code="404">Atleta não encontrado.</response>
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
