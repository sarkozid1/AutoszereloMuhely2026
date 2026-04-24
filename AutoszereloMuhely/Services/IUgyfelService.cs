using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Services;

// Ügyfél CRUD műveletek interfész
public interface IUgyfelService
{
    Task<List<UgyfelDto>> GetAllAsync();                              // Összes ügyfél lekérése
    Task<UgyfelDto?> GetByIdAsync(string id);                        // Egy ügyfél lekérése id alapján
    Task<UgyfelDto> CreateAsync(CreateUgyfelDto dto);                 // Új ügyfél létrehozása
    Task<UgyfelDto?> UpdateAsync(string id, CreateUgyfelDto dto);    // Ügyfél módosítása
    Task<bool> DeleteAsync(string id);                               // Ügyfél törlése
}