using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Services;

// Ügyfél CRUD műveletek interfész
public interface IUgyfelService
{
    Task<List<UgyfelDto>> GetAllAsync();                          // Összes ügyfél lekérése
    Task<UgyfelDto?> GetByIdAsync(int id);                       // Egy ügyfél lekérése id alapján
    Task<UgyfelDto> CreateAsync(CreateUgyfelDto dto);             // Új ügyfél létrehozása
    Task<UgyfelDto?> UpdateAsync(int id, CreateUgyfelDto dto);    // Ügyfél módosítása
    Task<bool> DeleteAsync(int id);                               // Ügyfél törlése
}