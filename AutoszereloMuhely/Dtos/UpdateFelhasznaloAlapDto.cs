using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Dtos;

// Dolgozó által szerkeszthető alapadatok - NEM tartalmaz bejelentkezési vagy szerepkör mezőket
public class UpdateFelhasznaloAlapDto
{
    [Required(ErrorMessage = "A név megadása kötelező.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
    public string Nev { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Érvényes telefonszámot adjon meg.")]
    public string? Telefonszam { get; set; }

    public string? Lakcim { get; set; }
}
