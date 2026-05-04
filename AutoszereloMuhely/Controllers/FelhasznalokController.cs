using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;

namespace AutoszereloMuhely.Controllers;

[ApiController]
[Route("api/felhasznalok")]
[Authorize]
public class FelhasznalokController : ControllerBase
{
    private readonly IFelhasznaloService _service;

    public FelhasznalokController(IFelhasznaloService service) => _service = service;

    // GET /api/felhasznalok - Csak adminisztrátor láthatja az összes felhasználót
    [HttpGet]
    [Authorize(Roles = "Adminisztrator")]
    public async Task<ActionResult<List<FelhasznaloDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET /api/felhasznalok/sajat - Bejelentkezett felhasználó saját adata
    [HttpGet("sajat")]
    public async Task<ActionResult<FelhasznaloDto>> GetSajat()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();
        var f = await _service.GetByIdAsync(id);
        return f == null ? NotFound() : Ok(f);
    }

    // PUT /api/felhasznalok/sajat - Bejelentkezett felhasználó saját alapadatait szerkeszti
    [HttpPut("sajat")]
    public async Task<ActionResult<FelhasznaloDto>> UpdateSajat(UpdateFelhasznaloAlapDto dto)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();
        var result = await _service.UpdateAlapAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // GET /api/felhasznalok/5 - Dolgozó és admin is lekérhet egy felhasználót
    [HttpGet("{id}")]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<FelhasznaloDto>> GetById(string id)
    {
        var f = await _service.GetByIdAsync(id);
        return f == null ? NotFound() : Ok(f);
    }

    // PUT /api/felhasznalok/5/alap - Alapadatok szerkesztése (dolgozó + admin)
    [HttpPut("{id}/alap")]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<FelhasznaloDto>> UpdateAlap(string id, UpdateFelhasznaloAlapDto dto)
    {
        var result = await _service.UpdateAlapAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // PUT /api/felhasznalok/5/szerep - Szerepkör módosítása (csak admin)
    [HttpPut("{id}/szerep")]
    [Authorize(Roles = "Adminisztrator")]
    public async Task<ActionResult<FelhasznaloDto>> UpdateSzerep(string id, UpdateSzerepDto dto)
    {
        var result = await _service.UpdateSzerepAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    // DELETE /api/felhasznalok/5 - Törlés (csak admin)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Adminisztrator")]
    public async Task<ActionResult> Delete(string id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
