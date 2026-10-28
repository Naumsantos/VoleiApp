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
            var atletas = dto.Atletas
                .Select(a => new TimeAtleta
                {
                    AtletaId = a.AtletaId
                })
                .ToList();

            var time = new Time
            {
                Nome = dto.Nome,
                Atletas = atletas
            };

            await _repository.AddAsync(time, ct);

            return new TimeResponseDTO { ID = time.ID, Nome = time.Nome };
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var time = await _repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Time {id} não encontrado.");

            await _repository.DeleteAsync(time, ct);
        }

        public async Task<IEnumerable<TimeCompletoResponseDTO>> GetAllAsync(CancellationToken ct = default)
        {
            var times = await _repository.GetAllAsync(ct);

            var response = times.Select(t => new TimeCompletoResponseDTO
            {
                ID = t.ID,
                Nome = t.Nome,
                Atletas = t.Atletas.Select(ta => new TimeAtletaResponseDTO
                {
                    ID = ta.ID,
                    TimeId = ta.TimeId,
                    Time = t.Nome,
                    AtletaId = ta.AtletaId,
                    Atleta = new()
                    {
                        ID = ta.Atleta.ID,
                        Nome = ta.Atleta.Nome,
                        Posicao = ta.Atleta.Posicao
                    }
                }).ToList()
            });

            return response;
        }

        public async Task<Time> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _repository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Time {id} não encontrado.");
        }

        public async Task UpdateAsync(int id, TimeDTO dto, CancellationToken ct = default)
        {
            var time = await _repository.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Time {id} não encontrado.");

            time.Nome = dto.Nome;

            await _repository.UpdateAsync(time, ct);
        }
    }
}
