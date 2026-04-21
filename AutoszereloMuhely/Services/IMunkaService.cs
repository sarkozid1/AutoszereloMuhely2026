using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Services;

public interface IMunkaService
{
    Task<List<MunkaDto>> GetAllAsync();
    Task<MunkaDto?> GetByIdAsync(int id);
    Task<List<MunkaDto>> GetByUgyfelIdAsync(int ugyfelId);
    Task<MunkaDto> CreateAsync(CreateMunkaDto dto);
    Task<MunkaDto?> UpdateAsync(int id, UpdateMunkaDto dto);
    Task<bool> DeleteAsync(int id);
    Task<MunkaDto?> UpdateAllapotAsync(int id, string ujAllapot);
}