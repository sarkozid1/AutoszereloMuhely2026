using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Services;

public interface IUgyfelService
{
    Task<List<UgyfelDto>> GetAllAsync();
    Task<UgyfelDto?> GetByIdAsync(int id);
    Task<UgyfelDto> CreateAsync(CreateUgyfelDto dto);
    Task<UgyfelDto?> UpdateAsync(int id, CreateUgyfelDto dto);
    Task<bool> DeleteAsync(int id);
}