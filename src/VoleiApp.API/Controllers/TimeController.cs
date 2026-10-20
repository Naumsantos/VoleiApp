using Microsoft.AspNetCore.Mvc;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Domain.Entities;

namespace VoleiApp.API.Controllers
{
    /// <summary>
    /// Endpoints para gerenciamento de times de vôlei.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TimeController : ControllerBase
    {
        private readonly ITimeService _service;

        public TimeController(ITimeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todos os times cadastrados com seus respectivos atletas.
        /// </summary>
        /// <returns>Lista de times.</returns>
        /// <response code="200">Times retornados com sucesso.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Time>>> GetAll(CancellationToken ct = default)
        {
            var times = await _service.GetAllAsync(ct);
            return Ok(times);
        }

        /// <summary>
        /// Busca um time pelo identificador.
        /// </summary>
        /// <param name="id">Identificador único do time.</param>
        /// <returns>Dados do time encontrado.</returns>
        /// <response code="200">Time encontrado com sucesso.</response>
        /// <response code="404">Time não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Time>> GetById(int id, CancellationToken ct = default)
        {
            var time = await _service.GetByIdAsync(id, ct);

            return time == null ? NotFound() : Ok(time);
        }

        /// <summary>
        /// Cria um novo time.
        /// </summary>
        /// <param name="time">Objeto com os dados do time a ser criado.</param>
        /// <returns>Time criado.</returns>
        /// <response code="201">Time criado com sucesso.</response>
        [HttpPost]
        public async Task<ActionResult<Time>> Create(CriarTimeDTO time, CancellationToken ct = default)
        {
            await _service.CreateAsync(time, ct);
            return CreatedAtAction(nameof(time.Nome), new { time.Nome }, time);
        }

        /// <summary>
        /// Atualiza os dados de um time existente.
        /// </summary>
        /// <param name="id">Identificador do time a ser atualizado.</param>
        /// <param name="time">Objeto com os dados atualizados do time.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Time atualizado com sucesso.</response>
        /// <response code="400">ID da rota diferente do ID informado no corpo da requisição.</response>
        [HttpPut]
        public async Task<IActionResult> Update(int id, TimeDTO time, CancellationToken ct = default)
        {
            if (id == 0) return BadRequest();

            await _service.UpdateAsync(id, time, ct);

            return NoContent();
        }

        /// <summary>
        /// Remove um time pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do time a ser removido.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Time removido com sucesso.</response>
        /// <response code="404">Time não encontrado.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            await _service.DeleteAsync(id, ct);

            return NoContent();
        }
    }
}
