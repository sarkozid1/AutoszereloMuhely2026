using Microsoft.EntityFrameworkCore;
using AutoszereloMuhely.Data;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Services;

// A munkák üzleti logikáját megvalósító service
// A DbContext-en keresztül kommunikál az adatbázissal
public class MunkaService : IMunkaService
{
    private readonly AppDbContext _context;           // Adatbázis elérés
    private readonly IMunkaoraService _munkaoraService; // Esztimáció számításhoz

    // Konstruktor - Dependency Injection-nel kapja meg a függőségeket
    public MunkaService(AppDbContext context, IMunkaoraService munkaoraService)
    {
        _context = context;
        _munkaoraService = munkaoraService;
    }

    // Összes munka lekérése az adatbázisból, DTO-vá alakítva
    public async Task<List<MunkaDto>> GetAllAsync()
    {
        var munkak = await _context.Munkak.ToListAsync();
        return munkak.Select(MapToDto).ToList();
    }

    // Egy munka lekérése id alapján - null-t ad vissza ha nem létezik
    public async Task<MunkaDto?> GetByIdAsync(int id)
    {
        var munka = await _context.Munkak.FindAsync(id);
        return munka == null ? null : MapToDto(munka);
    }

    // Egy adott ügyfélhez tartozó összes munka lekérése (megrendelői felülethez)
    public async Task<List<MunkaDto>> GetByUgyfelIdAsync(int ugyfelId)
    {
        var munkak = await _context.Munkak
            .Where(m => m.UgyfelId == ugyfelId) // Szűrés ügyfél id-re
            .ToListAsync();
        return munkak.Select(MapToDto).ToList();
    }

    // Új munka létrehozása - az állapot automatikusan FelvettMunka lesz
    public async Task<MunkaDto> CreateAsync(CreateMunkaDto dto)
    {
        // DTO-ból Munka entitás létrehozása
        var munka = new Munka
        {
            UgyfelId = dto.UgyfelId,
            Rendszam = dto.Rendszam,
            GyartasiEv = dto.GyartasiEv,
            Kategoria = Enum.Parse<MunkaKategoria>(dto.Kategoria), // String -> enum konverzió
            HibaLeiras = dto.HibaLeiras,
            HibaSulyossag = dto.HibaSulyossag,
            Allapot = MunkaAllapot.FelvettMunka // Új munka mindig "Felvett" állapotban indul
        };

        _context.Munkak.Add(munka);        // Hozzáadás a kontextushoz
        await _context.SaveChangesAsync();  // Mentés az adatbázisba
        return MapToDto(munka);             // Visszaadás DTO-ként
    }

    // Meglévő munka adatainak módosítása
    public async Task<MunkaDto?> UpdateAsync(int id, UpdateMunkaDto dto)
    {
        var munka = await _context.Munkak.FindAsync(id);
        if (munka == null) return null; // Ha nem létezik, null-t ad vissza

        // Mezők frissítése
        munka.Rendszam = dto.Rendszam;
        munka.GyartasiEv = dto.GyartasiEv;
        munka.Kategoria = Enum.Parse<MunkaKategoria>(dto.Kategoria);
        munka.HibaLeiras = dto.HibaLeiras;
        munka.HibaSulyossag = dto.HibaSulyossag;

        await _context.SaveChangesAsync();
        return MapToDto(munka);
    }

    // Állapot léptetése - CSAK előre lehet lépni (FelvettMunka -> ElvegzesAlatt -> Befejezett)
    public async Task<MunkaDto?> UpdateAllapotAsync(int id, string ujAllapot)
    {
        var munka = await _context.Munkak.FindAsync(id);
        if (munka == null) return null;

        var ujAllapotEnum = Enum.Parse<MunkaAllapot>(ujAllapot);

        // Ellenőrzés: az új állapot számértéke nagyobb kell legyen, mint a jelenlegi
        // Pl. FelvettMunka(0) -> ElvegzesAlatt(1) OK, de ElvegzesAlatt(1) -> FelvettMunka(0) TILOS
        if ((int)ujAllapotEnum <= (int)munka.Allapot)
            throw new InvalidOperationException("Az állapot csak előre léptethető.");

        munka.Allapot = ujAllapotEnum;
        await _context.SaveChangesAsync();
        return MapToDto(munka);
    }

    // Munka törlése - true ha sikerült, false ha nem létezett
    public async Task<bool> DeleteAsync(int id)
    {
        var munka = await _context.Munkak.FindAsync(id);
        if (munka == null) return false;

        _context.Munkak.Remove(munka);
        await _context.SaveChangesAsync();
        return true;
    }

    // Segédmetódus: Munka entitásból MunkaDto-t készít
    // Itt történik a munkaóra esztimáció kiszámítása is
    private MunkaDto MapToDto(Munka munka)
    {
        return new MunkaDto
        {
            Id = munka.Id,
            UgyfelId = munka.UgyfelId,
            Rendszam = munka.Rendszam,
            GyartasiEv = munka.GyartasiEv,
            Kategoria = munka.Kategoria.ToString(),   // Enum -> string (pl. "Motor")
            HibaLeiras = munka.HibaLeiras,
            HibaSulyossag = munka.HibaSulyossag,
            Allapot = munka.Allapot.ToString(),       // Enum -> string (pl. "FelvettMunka")
            MunkaoraEsztimacio = _munkaoraService.Szamol(
                munka.Kategoria.ToString(), munka.GyartasiEv, munka.HibaSulyossag)
        };
    }
}