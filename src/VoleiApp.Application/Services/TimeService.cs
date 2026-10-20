using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Interfaces.Repositories;

namespace VoleiApp.Application.Services
{
    public class TimeService : ITimeService
    {
        readonly ITimeRepository _repository;

        public TimeService(ITimeRepository repository)
        {
            _repository = repository;
        }

        public async Task<TimeResponseDTO> CreateAsync(CriarTimeDTO dto, CancellationToken ct = default)
        {
            var time = new Time { Nome = dto.Nome, Atletas = dto.Atletas };

            await _repository.AddAsync(time, ct);

            return new TimeResponseDTO { Nome = time.Nome };
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var time = await _repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Time {id} não encontrado.");

            await _repository.DeleteAsync(time, ct);
        }

        public async Task<IEnumerable<TimeDTO>> GetAllAsync(CancellationToken ct = default)
        {
            var times = await _repository.GetAllAsync(ct);

            return (IEnumerable<TimeDTO>)times.ToList();
        }

        public async Task<Time> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Time {id} não encontrado.");
        }

        public async Task UpdateAsync(int id, Time time, CancellationToken ct = default)
        {
            await _repository.GetByIdAsync(id, ct);

            await _repository.UpdateAsync(time, ct);
        }
    }
}
