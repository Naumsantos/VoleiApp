using Microsoft.EntityFrameworkCore;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Interfaces.Repositories;
using VoleiApp.Infrastructure.Persistence;

namespace VoleiApp.Infrastructure.Repositories
{
    public class AtletaRepository : IAtletaRepository
    {
        readonly VoleiContext _context;
        public AtletaRepository(VoleiContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Atleta atleta, CancellationToken ct = default)
        {
            var exist = await _context.Atletas.FirstOrDefaultAsync(a => a.ID == atleta.ID, ct);
            if (exist != null)
                throw new InvalidOperationException("Atleta já inserido");

            await _context.Atletas.AddAsync(atleta, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var atleta = await _context.Atletas.FirstOrDefaultAsync(a => a.ID == id, ct);

            if (atleta is null) return;

            _context.Atletas.Remove(atleta);
            await _context.SaveChangesAsync(ct);
        }

        public Task<List<Atleta>> GetAllAsync(CancellationToken ct = default)
        {
            return _context.Atletas.AsNoTracking().ToListAsync(ct);
        }

        public Task<Atleta?> GetByIdAsync(int id, CancellationToken ct = default) => _context.Atletas.FirstOrDefaultAsync(a => a.ID == id, ct);

        public async Task UpdateAsync(Atleta atleta, CancellationToken ct = default)
        {
            _context.Atletas.Update(atleta);
            await _context.SaveChangesAsync(ct);
        }
    }
}
