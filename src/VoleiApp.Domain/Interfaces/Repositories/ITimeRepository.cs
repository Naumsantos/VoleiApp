using VoleiApp.Domain.Entities;

namespace VoleiApp.Domain.Interfaces.Repositories
{
    public interface ITimeRepository
    {
        Task<List<Time>> GetAllAsync(CancellationToken ct);
        Task<Time?> GetByIdAsync(int id, CancellationToken ct);
        Task AddAsync(Time time, CancellationToken ct);
        Task UpdateAsync(Time time, CancellationToken ct);
        Task DeleteAsync(Time time, CancellationToken ct);
    }
}
