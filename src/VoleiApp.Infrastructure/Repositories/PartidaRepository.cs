using Microsoft.EntityFrameworkCore;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Interfaces.Repositories;
using VoleiApp.Infrastructure.Persistence;

namespace VoleiApp.Infrastructure.Repositories
{
    public class PartidaRepository : IPartidaRepository
    {
        readonly VoleiContext _context;

        public PartidaRepository(VoleiContext context) => _context = context;

        public async Task AddAsync(Partida partida, CancellationToken ct = default)
        {
            await _context.Partidas.AddAsync(partida, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task SavePartidaGraphAsync(Partida partida, CancellationToken ct = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            _context.Times.AddRange(partida.TimeA, partida.TimeB);
            await _context.SaveChangesAsync(ct);

            partida.TimeAId = partida.TimeA.ID;
            partida.TimeBId = partida.TimeB.ID;

            await _context.Partidas.AddAsync(partida, ct);
            await _context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
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

        public Task<List<Substituicao>> GetSubstituicoesAsync(CancellationToken ct = default)
            => _context.Substituicoes
                .Include(s => s.Entraram)
                .Include(s => s.Sairam)
                .AsNoTracking()
                .ToListAsync(ct);

        public Task<Substituicao?> GetSubstituicaoByIdAsync(int id, CancellationToken ct = default)
            => _context.Substituicoes
                .Include(s => s.Entraram)
                .Include(s => s.Sairam)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task UpdateAsync(Partida partida, CancellationToken ct = default)
        {
            _context.Update(partida);
            await _context.SaveChangesAsync(ct);
        }
    }
}
