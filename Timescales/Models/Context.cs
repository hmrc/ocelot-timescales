using Microsoft.EntityFrameworkCore;

namespace Timescales.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }


        public DbSet<Timescale> Timescales { get; set; }

        public DbSet<Audit> Audits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Timescale>()
                .HasIndex(t => t.Placeholder)
                .IsUnique();

            modelBuilder.Entity<Timescale>()
                .HasMany(t => t.Audit)
                .WithOne(t => t.Timescale)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Audit>()
                        .Property(a => a.User)
                        .IsFixedLength()
                        .IsUnicode(false);
        }
    }
}
