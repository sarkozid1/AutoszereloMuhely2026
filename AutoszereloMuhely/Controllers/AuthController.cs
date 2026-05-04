using Microsoft.AspNetCore.Mvc;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;

namespace AutoszereloMuhely.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IFelhasznaloService _service;

    public AuthController(IFelhasznaloService service) => _service = service;

    // POST /api/auth/regisztracio
    [HttpPost("regisztracio")]
    public async Task<ActionResult<TokenResponseDto>> Register(RegisterDto dto)
    {
        if (await _service.FelhasznalonevFoglaltAsync(dto.Felhasznalonev))
            return Conflict(new { message = "Ez a felhasználónév már foglalt." });

        if (await _service.EmailFoglaltAsync(dto.Email))
            return Conflict(new { message = "Ezzel az email címmel már létezik fiók." });

        var result = await _service.RegisterAsync(dto);
        return Ok(result);
    }

    // POST /api/auth/bejelentkezes
    [HttpPost("bejelentkezes")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var result = await _service.LoginAsync(dto);
        if (result == null)
            return Unauthorized(new { message = "Hibás felhasználónév vagy jelszó." });

        return Ok(result);
    }
}
