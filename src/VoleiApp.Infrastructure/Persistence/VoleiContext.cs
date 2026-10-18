using Microsoft.EntityFrameworkCore;
using VoleiApp.Domain.Entities;

namespace VoleiApp.Infrastructure.Persistence
{
    public class VoleiContext : DbContext
    {
        public VoleiContext(DbContextOptions<VoleiContext> options) : base(options) { }

        public DbSet<Atleta> Atletas { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<Substituicao> Substituicoes { get; set; }
        public DbSet<TimeAtleta> TimeAtletas { get; set; }
        public DbSet<PartidaReserva> PartidaReservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Partida>()
                .HasOne(p => p.TimeA)
                .WithMany()
                .HasForeignKey(p => p.TimeAId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partida>()
                .HasOne(p => p.TimeB)
                .WithMany()
                .HasForeignKey(p => p.TimeBId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partida>()
                .HasMany(p => p.Reservas)
                .WithOne(r => r.Partida)
                .HasForeignKey(p => p.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Partida>()
                .HasMany(p => p.Substituicoes)
                .WithOne(s => s.Partida)
                .HasForeignKey(p => p.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Time>()
                .HasMany(t => t.Atletas)
                .WithOne(ta => ta.Time)
                .HasForeignKey(t => t.TimeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TimeAtleta>()
                .HasOne(t => t.Atleta)
                .WithMany()
                .HasForeignKey(t => t.AtletaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PartidaReserva>()
                .HasOne(pr => pr.Atleta)
                .WithMany()
                .HasForeignKey(pr => pr.AtletaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Substituicao>()
                .HasOne(s => s.Time)
                .WithMany()
                .HasForeignKey(s => s.TimeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Substituicao>()
                .HasMany(s => s.Entraram)
                .WithMany();

            modelBuilder.Entity<Substituicao>()
                .HasMany(s => s.Sairam)
                .WithMany();

        }
    }
}
