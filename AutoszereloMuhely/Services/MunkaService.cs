using MongoDB.Driver;
using AutoszereloMuhely.Data;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Services;

// A munkák üzleti logikáját megvalósító service
// A MongoDbContext-en keresztül kommunikál az adatbázissal
public class MunkaService : IMunkaService
{
    private readonly IMongoCollection<Munka> _munkak;
    private readonly IMunkaoraService _munkaoraService;

    // Konstruktor - Dependency Injection-nel kapja meg a függőségeket
    public MunkaService(MongoDbContext context, IMunkaoraService munkaoraService)
    {
        _munkak = context.Munkak;
        _munkaoraService = munkaoraService;
    }

    // Összes munka lekérése az adatbázisból, DTO-vá alakítva
    public async Task<List<MunkaDto>> GetAllAsync()
    {
        var munkak = await _munkak.Find(_ => true).ToListAsync();
        return munkak.Select(MapToDto).ToList();
    }

    // Egy munka lekérése id alapján - null-t ad vissza ha nem létezik
    public async Task<MunkaDto?> GetByIdAsync(string id)
    {
        var munka = await _munkak.Find(m => m.Id == id).FirstOrDefaultAsync();
        return munka == null ? null : MapToDto(munka);
    }

    // Egy adott ügyfélhez tartozó összes munka lekérése (megrendelői felülethez)
    public async Task<List<MunkaDto>> GetByUgyfelIdAsync(string ugyfelId)
    {
        var munkak = await _munkak.Find(m => m.UgyfelId == ugyfelId).ToListAsync();
        return munkak.Select(MapToDto).ToList();
    }

    // Új munka létrehozása - az állapot automatikusan FelvettMunka lesz
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

        await _munkak.InsertOneAsync(munka);
        return MapToDto(munka);
    }

    // Meglévő munka adatainak módosítása
    public async Task<MunkaDto?> UpdateAsync(string id, UpdateMunkaDto dto)
    {
        var filter = Builders<Munka>.Filter.Eq(m => m.Id, id);
        var update = Builders<Munka>.Update
            .Set(m => m.Rendszam, dto.Rendszam)
            .Set(m => m.GyartasiEv, dto.GyartasiEv)
            .Set(m => m.Kategoria, Enum.Parse<MunkaKategoria>(dto.Kategoria))
            .Set(m => m.HibaLeiras, dto.HibaLeiras)
            .Set(m => m.HibaSulyossag, dto.HibaSulyossag);

        var result = await _munkak.FindOneAndUpdateAsync(
            filter, update, new FindOneAndUpdateOptions<Munka> { ReturnDocument = ReturnDocument.After });
        return result == null ? null : MapToDto(result);
    }

    // Állapot léptetése - CSAK előre lehet lépni (FelvettMunka -> ElvegzesAlatt -> Befejezett)
    public async Task<MunkaDto?> UpdateAllapotAsync(string id, string ujAllapot)
    {
        var munka = await _munkak.Find(m => m.Id == id).FirstOrDefaultAsync();
        if (munka == null) return null;

        var ujAllapotEnum = Enum.Parse<MunkaAllapot>(ujAllapot);
        if ((int)ujAllapotEnum <= (int)munka.Allapot)
            throw new InvalidOperationException("Az állapot csak előre léptethető.");

        var filter = Builders<Munka>.Filter.Eq(m => m.Id, id);
        var update = Builders<Munka>.Update.Set(m => m.Allapot, ujAllapotEnum);

        var result = await _munkak.FindOneAndUpdateAsync(
            filter, update, new FindOneAndUpdateOptions<Munka> { ReturnDocument = ReturnDocument.After });
        return result == null ? null : MapToDto(result);
    }

    // Munka törlése - true ha sikerült, false ha nem létezett
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _munkak.DeleteOneAsync(m => m.Id == id);
        return result.DeletedCount > 0;
    }

    // Egy ügyfél összes munkájának törlése (cascade delete helyett, mikor az ügyfél törlődik)
    public async Task DeleteByUgyfelIdAsync(string ugyfelId)
    {
        await _munkak.DeleteManyAsync(m => m.UgyfelId == ugyfelId);
    }

    // Segédmetódus: Munka entitásból MunkaDto-t készít
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