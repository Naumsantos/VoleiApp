using VoleiApp.Application.DTOs.Atleta;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Enums;
using VoleiApp.Domain.Interfaces.Repositories;

namespace VoleiApp.Application.Services
{
    public class AtletaService : IAtletaService
    {
        private readonly IAtletaRepository _repository;

        public AtletaService(IAtletaRepository repository) => _repository = repository;

        public async Task<AtletaResponseDTO> CreateAsync(CriarAtletaDTO dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Nome do atleta é obrigatório.");

            if (!Enum.TryParse<EPosicao>(dto.Posicao, ignoreCase: true, out var posicao))
                throw new ArgumentException($"Posição '{dto.Posicao}' inválida. Valores aceitos: {string.Join(", ", Enum.GetNames<EPosicao>())}");

            var atleta = new Atleta { Nome = dto.Nome, Posicao = posicao };
            await _repository.AddAsync(atleta, ct);
            return ToResponse(atleta);
        }
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var atleta = await _repository.GetByIdAsync(id, ct)
                    ?? throw new KeyNotFoundException($"Atleta {id} não encontrado.");

            await _repository.DeleteAsync(atleta.ID, ct);
        }

        public async Task<List<AtletaResponseDTO>> GetAllAsync(CancellationToken ct = default)
        {
            var atletas = await _repository.GetAllAsync(ct);
            return atletas.Select(ToResponse).ToList();
        }

        public async Task<AtletaResponseDTO?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var atleta = await _repository.GetByIdAsync(id, ct);
            return atleta is null ? null : ToResponse(atleta);
        }

        public async Task UpdateAsync(int id, AtualizarAtletaDTO dto, CancellationToken ct = default)
        {
            var atleta = await _repository.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Atleta {id} não encontrado.");

            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Nome do atleta é obrigatório.");

            if (!Enum.TryParse<EPosicao>(dto.Posicao, ignoreCase: true, out var posicao))
                throw new ArgumentException($"Posição '{dto.Posicao}' inválida. Valores aceitos: {string.Join(", ", Enum.GetNames<EPosicao>())}");

            atleta.Nome = dto.Nome;
            atleta.Posicao = posicao;
            await _repository.UpdateAsync(atleta, ct);
        }

        private static AtletaResponseDTO ToResponse(Atleta a) => new()
        {
            ID = a.ID,
            Nome = a.Nome,
            Posicao = a.Posicao
        };
    }
}
