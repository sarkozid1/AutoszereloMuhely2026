using MongoDB.Driver;
using AutoszereloMuhely.Data;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Services;

// Az ügyfelek üzleti logikáját megvalósító service
public class UgyfelService : IUgyfelService
{
    private readonly IMongoCollection<Ugyfel> _ugyfelek;
    private readonly IMunkaService _munkaService;

    // Konstruktor - DI-vel kapja meg a MongoDbContext-et és a MunkaService-t
    public UgyfelService(MongoDbContext context, IMunkaService munkaService)
    {
        _ugyfelek = context.Ugyfelek;
        _munkaService = munkaService;
    }

    // Összes ügyfél lekérése
    public async Task<List<UgyfelDto>> GetAllAsync()
    {
        var ugyfelek = await _ugyfelek.Find(_ => true).ToListAsync();
        return ugyfelek.Select(MapToDto).ToList();
    }

    // Egy ügyfél lekérése id alapján
    public async Task<UgyfelDto?> GetByIdAsync(string id)
    {
        var ugyfel = await _ugyfelek.Find(u => u.Id == id).FirstOrDefaultAsync();
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

        await _ugyfelek.InsertOneAsync(ugyfel);
        return MapToDto(ugyfel);
    }

    // Meglévő ügyfél adatainak módosítása
    public async Task<UgyfelDto?> UpdateAsync(string id, CreateUgyfelDto dto)
    {
        var filter = Builders<Ugyfel>.Filter.Eq(u => u.Id, id);
        var update = Builders<Ugyfel>.Update
            .Set(u => u.Nev, dto.Nev)
            .Set(u => u.Lakcim, dto.Lakcim)
            .Set(u => u.Email, dto.Email);

        var result = await _ugyfelek.FindOneAndUpdateAsync(
            filter, update, new FindOneAndUpdateOptions<Ugyfel> { ReturnDocument = ReturnDocument.After });
        return result == null ? null : MapToDto(result);
    }

    // Ügyfél törlése - a munkái is törlődnek (cascade)
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _ugyfelek.DeleteOneAsync(u => u.Id == id);
        if (result.DeletedCount > 0)
        {
            await _munkaService.DeleteByUgyfelIdAsync(id);
            return true;
        }
        return false;
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