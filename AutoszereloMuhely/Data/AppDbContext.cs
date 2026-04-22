using Microsoft.EntityFrameworkCore;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Data;

// Az Entity Framework adatbázis kontextus - ez a "kapu" az adatbázishoz
// Minden adatbázis művelet ezen keresztül történik
public class AppDbContext : DbContext
{
    // Konstruktor - a beállításokat (pl. SQLite connection string) kapja meg
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Az Ugyfelek tábla elérése - LINQ-val lekérdezhető
    public DbSet<Ugyfel> Ugyfelek => Set<Ugyfel>();

    // A Munkak tábla elérése
    public DbSet<Munka> Munkak => Set<Munka>();

    // Az adatbázis struktúra finomhangolása (Fluent API)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Munka-Ugyfel kapcsolat beállítása: 1 ügyfél -> több munka
        // Cascade törlés: ha az ügyfél törlődik, a munkái is törlődnek
        modelBuilder.Entity<Munka>()
            .HasOne(m => m.Ugyfel)
            .WithMany(u => u.Munkak)
            .HasForeignKey(m => m.UgyfelId)
            .OnDelete(DeleteBehavior.Cascade);

        // A Kategoria enum stringként tárolódik az adatbázisban (pl. "Motor")
        modelBuilder.Entity<Munka>()
            .Property(m => m.Kategoria)
            .HasConversion<string>();

        // Az Allapot enum stringként tárolódik (pl. "FelvettMunka")
        modelBuilder.Entity<Munka>()
            .Property(m => m.Allapot)
            .HasConversion<string>();
    }
}