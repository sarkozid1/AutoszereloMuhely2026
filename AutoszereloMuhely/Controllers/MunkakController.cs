using Microsoft.AspNetCore.Mvc;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;

namespace AutoszereloMuhely.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MunkakController : ControllerBase
{
    private readonly IMunkaService _service;

    public MunkakController(IMunkaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<MunkaDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<MunkaDto>> GetById(int id)
    {
        var munka = await _service.GetByIdAsync(id);
        return munka == null ? NotFound() : Ok(munka);
    }

    [HttpGet("ugyfel/{ugyfelId}")]
    public async Task<ActionResult<List<MunkaDto>>> GetByUgyfel(int ugyfelId)
        => Ok(await _service.GetByUgyfelIdAsync(ugyfelId));

    [HttpPost]
    public async Task<ActionResult<MunkaDto>> Create(CreateMunkaDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MunkaDto>> Update(int id, UpdateMunkaDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id}/allapot")]
    public async Task<ActionResult<MunkaDto>> UpdateAllapot(int id, [FromBody] string ujAllapot)
    {
        var result = await _service.UpdateAllapotAsync(id, ujAllapot);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}