using VoleiApp.Domain.Entities;

namespace VoleiApp.Domain.Interfaces.Repositories;

public interface IAtletaRepository
{
    Task<List<Atleta>> GetAllAsync(CancellationToken ct = default);
    Task<Atleta?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Atleta atleta, CancellationToken ct = default);
    Task UpdateAsync(Atleta atleta, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}