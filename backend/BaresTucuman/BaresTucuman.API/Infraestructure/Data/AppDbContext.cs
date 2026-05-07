using BaresTucuman.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaresTucuman.API.Infraestructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bar> Bares { get; set; }
    }
}