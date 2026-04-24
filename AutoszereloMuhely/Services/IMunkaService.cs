using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Services;

// Munka CRUD műveletek interfész
// A controller ezt az interfészt kapja meg Dependency Injection-nel
public interface IMunkaService
{
    Task<List<MunkaDto>> GetAllAsync();                                  // Összes munka lekérése
    Task<MunkaDto?> GetByIdAsync(string id);                             // Egy munka lekérése id alapján
    Task<List<MunkaDto>> GetByUgyfelIdAsync(string ugyfelId);            // Egy ügyfél összes munkája
    Task<MunkaDto> CreateAsync(CreateMunkaDto dto);                      // Új munka létrehozása
    Task<MunkaDto?> UpdateAsync(string id, UpdateMunkaDto dto);          // Munka módosítása
    Task<bool> DeleteAsync(string id);                                   // Munka törlése
    Task<MunkaDto?> UpdateAllapotAsync(string id, string ujAllapot);     // Állapot léptetése
    Task DeleteByUgyfelIdAsync(string ugyfelId);                         // Ügyfél munkainak törlése
}