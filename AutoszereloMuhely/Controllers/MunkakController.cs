using Microsoft.AspNetCore.Mvc;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;

namespace AutoszereloMuhely.Controllers;

// A munkákhoz tartozó API végpontokat kezelő controller
// [ApiController] - automatikus modell validáció és 400-as hiba érvénytelen adatnál
// [Route] - az URL: /api/munkak
[ApiController]
[Route("api/[controller]")]
public class MunkakController : ControllerBase
{
    private readonly IMunkaService _service;

    // DI-vel kapja meg a service-t - nem a konkrét MunkaService-t, hanem az interfészt
    public MunkakController(IMunkaService service) => _service = service;

    // GET /api/munkak - Összes munka listázása
    [HttpGet]
    public async Task<ActionResult<List<MunkaDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET /api/munkak/5 - Egy adott munka lekérése
    [HttpGet("{id}")]
    public async Task<ActionResult<MunkaDto>> GetById(string id)
    {
        var munka = await _service.GetByIdAsync(id);
        return munka == null ? NotFound() : Ok(munka); // 404 ha nem létezik, 200 ha igen
    }

    // GET /api/munkak/ugyfel/3 - Egy ügyfél összes munkája (megrendelői felülethez)
    [HttpGet("ugyfel/{ugyfelId}")]
    public async Task<ActionResult<List<MunkaDto>>> GetByUgyfel(string ugyfelId)
        => Ok(await _service.GetByUgyfelIdAsync(ugyfelId));

    // POST /api/munkak - Új munka létrehozása
    // 201 Created választ ad vissza az új munka elérhetőségével
    [HttpPost]
    public async Task<ActionResult<MunkaDto>> Create(CreateMunkaDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT /api/munkak/5 - Meglévő munka módosítása
    [HttpPut("{id}")]
    public async Task<ActionResult<MunkaDto>> Update(string id, UpdateMunkaDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // PATCH /api/munkak/5/allapot - Munka állapotának léptetése
    // Csak előre lehet: FelvettMunka -> ElvegzesAlatt -> Befejezett
    [HttpPatch("{id}/allapot")]
    public async Task<ActionResult<MunkaDto>> UpdateAllapot(string id, [FromBody] string ujAllapot)
    {
        var result = await _service.UpdateAllapotAsync(id, ujAllapot);
        return result == null ? NotFound() : Ok(result);
    }

    // DELETE /api/munkak/5 - Munka törlése
    // 204 NoContent ha sikerült, 404 ha nem létezett
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}