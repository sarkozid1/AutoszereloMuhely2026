namespace AutoszereloMuhely.Dtos;

// Felhasználó adatai - soha nem tartalmaz jelszót vagy hash-t
public class FelhasznaloDto
{
    public string Id { get; set; } = string.Empty;
    public string Felhasznalonev { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Szerep { get; set; } = string.Empty;
    public string Nev { get; set; } = string.Empty;
    public string? Telefonszam { get; set; }
    public string? Lakcim { get; set; }
    public string? UgyfelId { get; set; }
}
