using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Dtos;

// Anonim megrendelés - ügyfél adat + munka adat egyben (nem bejelentkezett felhasználónak)
public class AnonMegrendelesDto
{
    // Ügyfél adatok
    [Required(ErrorMessage = "A név megadása kötelező.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
    public string Nev { get; set; } = string.Empty;

    [Required(ErrorMessage = "Az email megadása kötelező.")]
    [EmailAddress(ErrorMessage = "Érvényes email cím szükséges.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A lakcím megadása kötelező.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
    public string Lakcim { get; set; } = string.Empty;

    // Jármű és hiba adatok
    [Required(ErrorMessage = "A rendszám megadása kötelező.")]
    [RegularExpression(@"^[A-Z]{3}-\d{3}$", ErrorMessage = "Rendszám formátuma: XXX-YYY (X: nagybetű, Y: szám)")]
    public string Rendszam { get; set; } = string.Empty;

    [Required]
    [Range(1900, int.MaxValue, ErrorMessage = "A gyártási év nem lehet kisebb 1900-nál.")]
    public int GyartasiEv { get; set; }

    [Required]
    public string Kategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A hiba leírása kötelező.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
    public string HibaLeiras { get; set; } = string.Empty;

    [Required]
    [Range(1, 10, ErrorMessage = "A súlyosság 1 és 10 között legyen.")]
    public int HibaSulyossag { get; set; }
}
