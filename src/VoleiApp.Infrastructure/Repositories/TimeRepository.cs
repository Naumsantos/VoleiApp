using Microsoft.EntityFrameworkCore;
using VoleiApp.Domain.Entities;
using VoleiApp.Domain.Interfaces.Repositories;
using VoleiApp.Infrastructure.Persistence;

namespace VoleiApp.Infrastructure.Repositories
{
    public class TimeRepository : ITimeRepository
    {
        readonly VoleiContext _context;

        public TimeRepository(VoleiContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Time time, CancellationToken ct = default)
        {
            await _context.Times.AddAsync(time);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Time time, CancellationToken ct = default)
        {
            _context.Times.Remove(time);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Time>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Times
                .Include(t => t.Atletas)
                .ThenInclude(ta => ta.Atleta)
                .ToListAsync(ct);
        }

        public async Task<Time?> GetByIdAsync(int id, CancellationToken ct = default) => await _context.Times
            .Include(t => t.Atletas)
            .ThenInclude(ta => ta.Atleta)
            .FirstOrDefaultAsync(t => t.ID == id, ct);

        public async Task UpdateAsync(Time time, CancellationToken ct = default)
        {
            _context.Times.Update(time);
            await _context.SaveChangesAsync(ct);
        }
    }
}
