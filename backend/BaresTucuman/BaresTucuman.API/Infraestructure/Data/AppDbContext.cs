using BaresTucuman.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaresTucuman.API.Infraestructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bar> Bares { get; set; }
        public DbSet<SyncLog> SyncLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bar>()
                .Property(b => b.CategoriaAMostrar)
                .HasConversion<string>();
        }
    }
}