using Microsoft.EntityFrameworkCore;
using AutoszereloMuhely.Data;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Services;

public class MunkaService : IMunkaService
{
    private readonly AppDbContext _context;
    private readonly IMunkaoraService _munkaoraService;

    public MunkaService(AppDbContext context, IMunkaoraService munkaoraService)
    {
        _context = context;
        _munkaoraService = munkaoraService;
    }

    public async Task<List<MunkaDto>> GetAllAsync()
    {
        var munkak = await _context.Munkak.ToListAsync();
        return munkak.Select(MapToDto).ToList();
    }

    public async Task<MunkaDto?> GetByIdAsync(int id)
    {
        var munka = await _context.Munkak.FindAsync(id);
        return munka == null ? null : MapToDto(munka);
    }

    public async Task<List<MunkaDto>> GetByUgyfelIdAsync(int ugyfelId)
    {
        var munkak = await _context.Munkak
            .Where(m => m.UgyfelId == ugyfelId)
            .ToListAsync();
        return munkak.Select(MapToDto).ToList();
    }

    public async Task<MunkaDto> CreateAsync(CreateMunkaDto dto)
    {
        var munka = new Munka
        {
            UgyfelId = dto.UgyfelId,
            Rendszam = dto.Rendszam,
            GyartasiEv = dto.GyartasiEv,
            Kategoria = Enum.Parse<MunkaKategoria>(dto.Kategoria),
            HibaLeiras = dto.HibaLeiras,
            HibaSulyossag = dto.HibaSulyossag,
            Allapot = MunkaAllapot.FelvettMunka
        };

        _context.Munkak.Add(munka);
        await _context.SaveChangesAsync();
        return MapToDto(munka);
    }

    public async Task<MunkaDto?> UpdateAsync(int id, UpdateMunkaDto dto)
    {
        var munka = await _context.Munkak.FindAsync(id);
        if (munka == null) return null;

        munka.Rendszam = dto.Rendszam;
        munka.GyartasiEv = dto.GyartasiEv;
        munka.Kategoria = Enum.Parse<MunkaKategoria>(dto.Kategoria);
        munka.HibaLeiras = dto.HibaLeiras;
        munka.HibaSulyossag = dto.HibaSulyossag;

        await _context.SaveChangesAsync();
        return MapToDto(munka);
    }

    public async Task<MunkaDto?> UpdateAllapotAsync(int id, string ujAllapot)
    {
        var munka = await _context.Munkak.FindAsync(id);
        if (munka == null) return null;

        var ujAllapotEnum = Enum.Parse<MunkaAllapot>(ujAllapot);

        if ((int)ujAllapotEnum <= (int)munka.Allapot)
            throw new InvalidOperationException("Az állapot csak előre léptethető.");

        munka.Allapot = ujAllapotEnum;
        await _context.SaveChangesAsync();
        return MapToDto(munka);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var munka = await _context.Munkak.FindAsync(id);
        if (munka == null) return false;

        _context.Munkak.Remove(munka);
        await _context.SaveChangesAsync();
        return true;
    }

    private MunkaDto MapToDto(Munka munka)
    {
        return new MunkaDto
        {
            Id = munka.Id,
            UgyfelId = munka.UgyfelId,
            Rendszam = munka.Rendszam,
            GyartasiEv = munka.GyartasiEv,
            Kategoria = munka.Kategoria.ToString(),
            HibaLeiras = munka.HibaLeiras,
            HibaSulyossag = munka.HibaSulyossag,
            Allapot = munka.Allapot.ToString(),
            MunkaoraEsztimacio = _munkaoraService.Szamol(
                munka.Kategoria.ToString(), munka.GyartasiEv, munka.HibaSulyossag)
        };
    }
}