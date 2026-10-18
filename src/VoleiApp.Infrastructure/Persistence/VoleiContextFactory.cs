using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VoleiApp.Infrastructure.Persistence;

public class VoleiContextFactory : IDesignTimeDbContextFactory<VoleiContext>
{
    public VoleiContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VoleiContext>();
        optionsBuilder.UseSqlite("Data Source=voleiapp.db");

        return new VoleiContext(optionsBuilder.Options);
    }
}