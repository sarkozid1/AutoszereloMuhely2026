using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Services;

// Munka CRUD műveletek interfész
// A controller ezt az interfészt kapja meg Dependency Injection-nel
public interface IMunkaService
{
    Task<List<MunkaDto>> GetAllAsync();                              // Összes munka lekérése
    Task<MunkaDto?> GetByIdAsync(int id);                            // Egy munka lekérése id alapján
    Task<List<MunkaDto>> GetByUgyfelIdAsync(int ugyfelId);           // Egy ügyfél összes munkája
    Task<MunkaDto> CreateAsync(CreateMunkaDto dto);                  // Új munka létrehozása
    Task<MunkaDto?> UpdateAsync(int id, UpdateMunkaDto dto);         // Munka módosítása
    Task<bool> DeleteAsync(int id);                                  // Munka törlése
    Task<MunkaDto?> UpdateAllapotAsync(int id, string ujAllapot);    // Állapot léptetése
}