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
                .Include(p => p.TimeA)
                    .ThenInclude(t => t.Atletas)
                        .ThenInclude(ta => ta.Atleta)
                .Include(p => p.TimeB)
                    .ThenInclude(t => t.Atletas)
                        .ThenInclude(ta => ta.Atleta)
                .Include(r => r.Reservas)
                    .ThenInclude(r => r.Atleta)
                .Include(s => s.Substituicoes)
                    .ThenInclude(se => se.Entraram)
                .Include(s => s.Substituicoes)
                    .ThenInclude(ss => ss.Sairam)
                .AsNoTracking()
                .ToListAsync(ct);


        public Task<Partida?> GetByIdAsync(int id, CancellationToken ct = default)
            => _context.Partidas
                    .Include(p => p.TimeA)
                .ThenInclude(t => t.Atletas)
                    .ThenInclude(ta => ta.Atleta)
            .Include(p => p.TimeB)
                .ThenInclude(t => t.Atletas)
                    .ThenInclude(ta => ta.Atleta)
            .Include(p => p.Reservas.OrderBy(r => r.Ordem))
                .ThenInclude(r => r.Atleta)
            .Include(p => p.Substituicoes)
                .ThenInclude(s => s.Entraram)
            .Include(p => p.Substituicoes)
                .ThenInclude(s => s.Sairam)
            .FirstOrDefaultAsync(p => p.ID == id, ct);

        public async Task UpdateAsync(Partida partida, CancellationToken ct = default)
        {
            _context.Partidas.Update(partida);
            await _context.SaveChangesAsync(ct);
        }
    }
}
