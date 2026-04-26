using Microsoft.EntityFrameworkCore;
using VoleiApp.Models;


namespace VoleiApp.Data
{
    public class VoleiContext : DbContext
    {
        public VoleiContext(DbContextOptions<VoleiContext> options) : base(options) { }
        public DbSet<Atleta> Atletas { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<Substituicao> Substituicoes { get; set; }
    }
}
