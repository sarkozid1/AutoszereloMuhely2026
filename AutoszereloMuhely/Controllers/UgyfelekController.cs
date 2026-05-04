using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;

namespace AutoszereloMuhely.Controllers;

// Az ügyfelekhez tartozó API végpontokat kezelő controller
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dolgozo,Adminisztrator")]
public class UgyfelekController : ControllerBase
{
    private readonly IUgyfelService _service;

    public UgyfelekController(IUgyfelService service) => _service = service;

    // GET /api/ugyfelek - Összes ügyfél listázása
    [HttpGet]
    public async Task<ActionResult<List<UgyfelDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET /api/ugyfelek/5 - Egy ügyfél lekérése
    [HttpGet("{id}")]
    public async Task<ActionResult<UgyfelDto>> GetById(string id)
    {
        var ugyfel = await _service.GetByIdAsync(id);
        return ugyfel == null ? NotFound() : Ok(ugyfel);
    }

    // POST /api/ugyfelek - Új ügyfél létrehozása
    [HttpPost]
    public async Task<ActionResult<UgyfelDto>> Create(CreateUgyfelDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT /api/ugyfelek/5 - Ügyfél adatainak módosítása
    [HttpPut("{id}")]
    public async Task<ActionResult<UgyfelDto>> Update(string id, CreateUgyfelDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // DELETE /api/ugyfelek/5 - Ügyfél törlése (a munkái is törlődnek - cascade)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Adminisztrator")]
    public async Task<ActionResult> Delete(string id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}