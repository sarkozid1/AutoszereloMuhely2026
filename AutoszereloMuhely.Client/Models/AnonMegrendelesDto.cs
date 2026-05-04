using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Client.Models;

public class AnonMegrendelesDto
{
    [Required(ErrorMessage = "A név megadása kötelező.")]
    public string Nev { get; set; } = string.Empty;

    [Required(ErrorMessage = "Az email megadása kötelező.")]
    [EmailAddress(ErrorMessage = "Érvényes email cím szükséges.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A lakcím megadása kötelező.")]
    public string Lakcim { get; set; } = string.Empty;

    [Required(ErrorMessage = "A rendszám megadása kötelező.")]
    [RegularExpression(@"^[A-Z]{3}-\d{3}$", ErrorMessage = "Rendszám formátuma: XXX-YYY")]
    public string Rendszam { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100, ErrorMessage = "Érvényes gyártási évet adjon meg.")]
    public int GyartasiEv { get; set; } = DateTime.Now.Year;

    [Required]
    public string Kategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A hiba leírása kötelező.")]
    public string HibaLeiras { get; set; } = string.Empty;

    [Required]
    [Range(1, 10, ErrorMessage = "A súlyosság 1-10 között legyen.")]
    public int HibaSulyossag { get; set; } = 5;
}
