using Microsoft.EntityFrameworkCore;
using AutoszereloMuhely.Data;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Services;

// Az ügyfelek üzleti logikáját megvalósító service
public class UgyfelService : IUgyfelService
{
    private readonly AppDbContext _context;

    // Konstruktor - DI-vel kapja meg a DbContext-et
    public UgyfelService(AppDbContext context)
    {
        _context = context;
    }

    // Összes ügyfél lekérése
    public async Task<List<UgyfelDto>> GetAllAsync()
    {
        var ugyfelek = await _context.Ugyfelek.ToListAsync();
        return ugyfelek.Select(MapToDto).ToList();
    }

    // Egy ügyfél lekérése id alapján
    public async Task<UgyfelDto?> GetByIdAsync(int id)
    {
        var ugyfel = await _context.Ugyfelek.FindAsync(id);
        return ugyfel == null ? null : MapToDto(ugyfel);
    }

    // Új ügyfél létrehozása
    public async Task<UgyfelDto> CreateAsync(CreateUgyfelDto dto)
    {
        var ugyfel = new Ugyfel
        {
            Nev = dto.Nev,
            Lakcim = dto.Lakcim,
            Email = dto.Email
        };

        _context.Ugyfelek.Add(ugyfel);
        await _context.SaveChangesAsync();
        return MapToDto(ugyfel);
    }

    // Meglévő ügyfél adatainak módosítása
    public async Task<UgyfelDto?> UpdateAsync(int id, CreateUgyfelDto dto)
    {
        var ugyfel = await _context.Ugyfelek.FindAsync(id);
        if (ugyfel == null) return null;

        ugyfel.Nev = dto.Nev;
        ugyfel.Lakcim = dto.Lakcim;
        ugyfel.Email = dto.Email;

        await _context.SaveChangesAsync();
        return MapToDto(ugyfel);
    }

    // Ügyfél törlése
    public async Task<bool> DeleteAsync(int id)
    {
        var ugyfel = await _context.Ugyfelek.FindAsync(id);
        if (ugyfel == null) return false;

        _context.Ugyfelek.Remove(ugyfel);
        await _context.SaveChangesAsync();
        return true;
    }

    // Segédmetódus: Ugyfel entitásból UgyfelDto-t készít
    private UgyfelDto MapToDto(Ugyfel ugyfel)
    {
        return new UgyfelDto
        {
            Id = ugyfel.Id,
            Nev = ugyfel.Nev,
            Lakcim = ugyfel.Lakcim,
            Email = ugyfel.Email
        };
    }
}