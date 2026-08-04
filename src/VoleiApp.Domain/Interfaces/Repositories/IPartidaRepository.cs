using VoleiApp.Domain.Entities;

namespace VoleiApp.Domain.Interfaces.Repositories;

public interface IPartidaRepository
{
    Task<List<Partida>> GetAllAsync(CancellationToken ct = default);
    Task<Partida?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Partida partida, CancellationToken ct = default);

}