using Microsoft.EntityFrameworkCore;

namespace Timescales.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        { }

        public DbSet<Timescale> Timescales { get; set; }

        public DbSet<Audit> Audits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Timescale>()
                .HasIndex(t => t.Placeholder)
                .IsUnique();

            modelBuilder.Entity<Timescale>()
                .HasMany(e => e.Audit)
                .WithOne(e => e.Timescale)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
