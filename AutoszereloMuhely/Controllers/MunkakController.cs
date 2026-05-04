using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoszereloMuhely.Data;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Models;
using AutoszereloMuhely.Services;
using MongoDB.Driver;

namespace AutoszereloMuhely.Controllers;

// A munkákhoz tartozó API végpontokat kezelő controller
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MunkakController : ControllerBase
{
    private readonly IMunkaService _service;
    private readonly MongoDbContext _context;

    public MunkakController(IMunkaService service, MongoDbContext context)
    {
        _service = service;
        _context = context;
    }

    // GET /api/munkak - Összes munka listázása
    [HttpGet]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<List<MunkaDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET /api/munkak/5 - Egy adott munka lekérése
    [HttpGet("{id}")]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<MunkaDto>> GetById(string id)
    {
        var munka = await _service.GetByIdAsync(id);
        return munka == null ? NotFound() : Ok(munka);
    }

    // GET /api/munkak/ugyfel/3 - Egy ügyfél összes munkája (dolgozó + admin)
    [HttpGet("ugyfel/{ugyfelId}")]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<List<MunkaDto>>> GetByUgyfel(string ugyfelId)
        => Ok(await _service.GetByUgyfelIdAsync(ugyfelId));

    // GET /api/munkak/sajat - A bejelentkezett felhasználó saját munkái
    [HttpGet("sajat")]
    [Authorize(Roles = "Felhasznalo")]
    public async Task<ActionResult<List<MunkaDto>>> GetSajat()
    {
        var ugyfelId = User.FindFirstValue("ugyfelid");
        if (string.IsNullOrEmpty(ugyfelId))
            return Ok(new List<MunkaDto>());
        return Ok(await _service.GetByUgyfelIdAsync(ugyfelId));
    }

    // POST /api/munkak - Új munka létrehozása (dolgozó/admin)
    [HttpPost]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<MunkaDto>> Create(CreateMunkaDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // POST /api/munkak/megrendeles - Anonim megrendelés leadása (bejelentkezés nélkül)
    [HttpPost("megrendeles")]
    [AllowAnonymous]
    public async Task<ActionResult<MunkaDto>> AnonMegrendeles(AnonMegrendelesDto dto)
    {
        // Meglévő ügyfél keresése email alapján, vagy új létrehozása
        var ugyfel = await _context.Ugyfelek
            .Find(u => u.Email == dto.Email)
            .FirstOrDefaultAsync();

        if (ugyfel == null)
        {
            ugyfel = new Ugyfel { Nev = dto.Nev, Email = dto.Email, Lakcim = dto.Lakcim };
            await _context.Ugyfelek.InsertOneAsync(ugyfel);
        }

        var munka = new CreateMunkaDto
        {
            UgyfelId = ugyfel.Id,
            Rendszam = dto.Rendszam,
            GyartasiEv = dto.GyartasiEv,
            Kategoria = dto.Kategoria,
            HibaLeiras = dto.HibaLeiras,
            HibaSulyossag = dto.HibaSulyossag
        };

        var result = await _service.CreateAsync(munka);
        return Ok(result);
    }

    // POST /api/munkak/sajat - Bejelentkezett felhasználó new munkát ad le
    [HttpPost("sajat")]
    [Authorize(Roles = "Felhasznalo")]
    public async Task<ActionResult<MunkaDto>> CreateSajat(SajatMunkaDto sajatDto)
    {
        var ugyfelId = User.FindFirstValue("ugyfelid");
        if (string.IsNullOrEmpty(ugyfelId))
            return BadRequest(new { message = "Nincs kapcsolódó ügyfélrekord." });

        var dto = new CreateMunkaDto
        {
            UgyfelId = ugyfelId,
            Rendszam = sajatDto.Rendszam,
            GyartasiEv = sajatDto.GyartasiEv,
            Kategoria = sajatDto.Kategoria,
            HibaLeiras = sajatDto.HibaLeiras,
            HibaSulyossag = sajatDto.HibaSulyossag
        };
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    // PUT /api/munkak/5 - Meglévő munka módosítása
    [HttpPut("{id}")]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<MunkaDto>> Update(string id, UpdateMunkaDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // PATCH /api/munkak/5/allapot - Munka állapotának léptetése
    [HttpPatch("{id}/allapot")]
    [Authorize(Roles = "Dolgozo,Adminisztrator")]
    public async Task<ActionResult<MunkaDto>> UpdateAllapot(string id, [FromBody] string ujAllapot)
    {
        var result = await _service.UpdateAllapotAsync(id, ujAllapot);
        return result == null ? NotFound() : Ok(result);
    }

    // DELETE /api/munkak/5 - Munka törlése (csak admin)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Adminisztrator")]
    public async Task<ActionResult> Delete(string id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}