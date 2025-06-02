using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoleiApp.Models;


namespace VoleiApp.Data
{
    public class VoleiContext : DbContext
    {
        public VoleiContext(DbContextOptions<VoleiContext> options) : base(options) { }
        public DbSet<Atleta> Atletas { get; set; }
        public DbSet<Partida> Partidas { get; set; }
    }
}
