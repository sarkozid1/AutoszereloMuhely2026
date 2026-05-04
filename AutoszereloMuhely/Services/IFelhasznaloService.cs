using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Services;

public interface IFelhasznaloService
{
    Task<TokenResponseDto> RegisterAsync(RegisterDto dto);
    Task<TokenResponseDto?> LoginAsync(LoginDto dto);
    Task<List<FelhasznaloDto>> GetAllAsync();
    Task<FelhasznaloDto?> GetByIdAsync(string id);
    Task<FelhasznaloDto?> UpdateAlapAsync(string id, UpdateFelhasznaloAlapDto dto);
    Task<FelhasznaloDto?> UpdateSzerepAsync(string id, UpdateSzerepDto dto);
    Task<bool> DeleteAsync(string id);
    Task<bool> FelhasznalonevFoglaltAsync(string felhasznalonev);
    Task<bool> EmailFoglaltAsync(string email);
}
