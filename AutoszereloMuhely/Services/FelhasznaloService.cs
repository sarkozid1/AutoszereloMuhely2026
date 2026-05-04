using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using AutoszereloMuhely.Data;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Services;

public class FelhasznaloService : IFelhasznaloService
{
    private readonly IMongoCollection<Felhasznalo> _felhasznalok;
    private readonly IMongoCollection<Ugyfel> _ugyfelek;
    private readonly JwtSettings _jwtSettings;

    public FelhasznaloService(MongoDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _felhasznalok = context.Felhasznalok;
        _ugyfelek = context.Ugyfelek;
        _jwtSettings = jwtSettings.Value;
    }

    // Regisztráció: létrehozza a Felhasznalo-t és az ahhoz tartozó Ugyfel rekordot
    public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto)
    {
        var ugyfel = new Ugyfel
        {
            Nev = dto.Nev,
            Email = dto.Email,
            Lakcim = string.Empty
        };
        await _ugyfelek.InsertOneAsync(ugyfel);

        var felhasznalo = new Felhasznalo
        {
            Felhasznalonev = dto.Felhasznalonev,
            Email = dto.Email,
            JelszoHash = HashPassword(dto.Jelszo),
            Szerep = FelhasznaloSzerep.Felhasznalo,
            Nev = dto.Nev,
            Lakcim = dto.Lakcim,
            UgyfelId = ugyfel.Id
        };
        await _felhasznalok.InsertOneAsync(felhasznalo);

        return new TokenResponseDto
        {
            Token = GenerateToken(felhasznalo),
            Felhasznalo = MapToDto(felhasznalo)
        };
    }

    // Bejelentkezés
    public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
    {
        var felhasznalo = await _felhasznalok
            .Find(f => f.Felhasznalonev == dto.Felhasznalonev)
            .FirstOrDefaultAsync();

        if (felhasznalo == null || !VerifyPassword(dto.Jelszo, felhasznalo.JelszoHash))
            return null;

        return new TokenResponseDto
        {
            Token = GenerateToken(felhasznalo),
            Felhasznalo = MapToDto(felhasznalo)
        };
    }

    public async Task<List<FelhasznaloDto>> GetAllAsync()
    {
        var lista = await _felhasznalok.Find(_ => true).ToListAsync();
        return lista.Select(MapToDto).ToList();
    }

    public async Task<FelhasznaloDto?> GetByIdAsync(string id)
    {
        var f = await _felhasznalok.Find(f => f.Id == id).FirstOrDefaultAsync();
        return f == null ? null : MapToDto(f);
    }

    // Alapadatok frissítése (dolgozó hozzáférés): csak nev és telefonszam
    public async Task<FelhasznaloDto?> UpdateAlapAsync(string id, UpdateFelhasznaloAlapDto dto)
    {
        var filter = Builders<Felhasznalo>.Filter.Eq(f => f.Id, id);
        var update = Builders<Felhasznalo>.Update
            .Set(f => f.Nev, dto.Nev)
            .Set(f => f.Telefonszam, dto.Telefonszam)
            .Set(f => f.Lakcim, dto.Lakcim);

        var result = await _felhasznalok.FindOneAndUpdateAsync(
            filter, update, new FindOneAndUpdateOptions<Felhasznalo> { ReturnDocument = ReturnDocument.After });

        if (result == null) return null;

        // Ha van kapcsolódó Ugyfel rekord, szinkronizálja a nevet és lakcímet ott is
        if (!string.IsNullOrEmpty(result.UgyfelId))
        {
            var ugyfelUpdate = Builders<Ugyfel>.Update
                .Set(u => u.Nev, dto.Nev)
                .Set(u => u.Lakcim, dto.Lakcim ?? string.Empty);
            await _ugyfelek.UpdateOneAsync(u => u.Id == result.UgyfelId, ugyfelUpdate);
        }

        return MapToDto(result);
    }

    // Szerepkör módosítása (csak admin)
    public async Task<FelhasznaloDto?> UpdateSzerepAsync(string id, UpdateSzerepDto dto)
    {
        if (!Enum.TryParse<FelhasznaloSzerep>(dto.Szerep, out var ujSzerep))
            return null;

        var filter = Builders<Felhasznalo>.Filter.Eq(f => f.Id, id);
        var update = Builders<Felhasznalo>.Update.Set(f => f.Szerep, ujSzerep);

        var result = await _felhasznalok.FindOneAndUpdateAsync(
            filter, update, new FindOneAndUpdateOptions<Felhasznalo> { ReturnDocument = ReturnDocument.After });
        return result == null ? null : MapToDto(result);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _felhasznalok.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> FelhasznalonevFoglaltAsync(string felhasznalonev)
        => await _felhasznalok.Find(f => f.Felhasznalonev == felhasznalonev).AnyAsync();

    public async Task<bool> EmailFoglaltAsync(string email)
        => await _felhasznalok.Find(f => f.Email == email).AnyAsync();

    // --- JWT token generálás ---
    private string GenerateToken(Felhasznalo felhasznalo)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, felhasznalo.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, felhasznalo.Felhasznalonev),
            new Claim(JwtRegisteredClaimNames.Email, felhasznalo.Email),
            new Claim(ClaimTypes.Role, felhasznalo.Szerep.ToString()),
            new Claim("nev", felhasznalo.Nev),
            new Claim("ugyfelid", felhasznalo.UgyfelId ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // --- Jelszó hash PBKDF2-vel ---
    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private static FelhasznaloDto MapToDto(Felhasznalo f) => new()
    {
        Id = f.Id,
        Felhasznalonev = f.Felhasznalonev,
        Email = f.Email,
        Szerep = f.Szerep.ToString(),
        Nev = f.Nev,
        Telefonszam = f.Telefonszam,
        Lakcim = f.Lakcim,
        UgyfelId = f.UgyfelId
    };
}

// JWT beállítások osztály (appsettings.json-ból)
public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
