using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Client.Models;

public class RegisterDto
{
    [Required(ErrorMessage = "A felhasználónév megadása kötelező.")]
    public string Felhasznalonev { get; set; } = string.Empty;

    [Required(ErrorMessage = "Az email megadása kötelező.")]
    [EmailAddress(ErrorMessage = "Érvényes email cím szükséges.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A jelszó megadása kötelező.")]
    [MinLength(6, ErrorMessage = "A jelszó legalább 6 karakter legyen.")]
    public string Jelszo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A jelszó megerősítése kötelező.")]
    [Compare(nameof(Jelszo), ErrorMessage = "A két jelszó nem egyezik.")]
    public string JelszoMegerosites { get; set; } = string.Empty;

    [Required(ErrorMessage = "A név megadása kötelező.")]
    public string Nev { get; set; } = string.Empty;

    public string? Lakcim { get; set; }
}
