using Microsoft.EntityFrameworkCore;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ugyfel> Ugyfelek => Set<Ugyfel>();
    public DbSet<Munka> Munkak => Set<Munka>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Munka>()
            .HasOne(m => m.Ugyfel)
            .WithMany(u => u.Munkak)
            .HasForeignKey(m => m.UgyfelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Munka>()
            .Property(m => m.Kategoria)
            .HasConversion<string>();

        modelBuilder.Entity<Munka>()
            .Property(m => m.Allapot)
            .HasConversion<string>();
    }
}