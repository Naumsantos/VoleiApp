using Microsoft.EntityFrameworkCore;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Interfaces.Repositories;

namespace VoleiApp.Infrastructure.Persistence
{
    public class PartidaRepository : IPartidaRepository
    {
        readonly VoleiContext _context;

        public PartidaRepository(VoleiContext context) => _context = context;

        public async Task AddAsync(Partida partida, CancellationToken ct = default)
        {
            await _context.Partidas.AddAsync(partida);
            await _context.SaveChangesAsync(ct);
        }

        public Task<List<Partida>> GetAllAsync(CancellationToken ct = default)
            => _context.Partidas
                .Include(p => p.TimeA).ThenInclude(t => t.Atletas)
                .Include(p => p.TimeB).ThenInclude(t => t.Atletas)
                .Include(p => p.Substituicoes)
                .AsNoTracking()
                .ToListAsync(ct);


        public Task<Partida?> GetByIdAsync(int id, CancellationToken ct = default)
            => _context.Partidas
                    .Include(p => p.TimeA).ThenInclude(t => t.Atletas)
                    .Include(p => p.TimeB).ThenInclude(t => t.Atletas)
                    .Include(p => p.Substituicoes)
                    .FirstOrDefaultAsync(p => p.ID == id, ct);
    }
}
