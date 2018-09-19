using Microsoft.EntityFrameworkCore;

namespace Timescales.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        { }

        public DbSet<Timescale> Timescales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Timescale>()
                .HasIndex(t => t.Placeholder)
                .IsUnique();
        }
    }
}
