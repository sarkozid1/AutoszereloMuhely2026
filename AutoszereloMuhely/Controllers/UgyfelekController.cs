using Microsoft.AspNetCore.Mvc;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;

namespace AutoszereloMuhely.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UgyfelekController : ControllerBase
{
    private readonly IUgyfelService _service;

    public UgyfelekController(IUgyfelService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<UgyfelDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<UgyfelDto>> GetById(int id)
    {
        var ugyfel = await _service.GetByIdAsync(id);
        return ugyfel == null ? NotFound() : Ok(ugyfel);
    }

    [HttpPost]
    public async Task<ActionResult<UgyfelDto>> Create(CreateUgyfelDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UgyfelDto>> Update(int id, CreateUgyfelDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}