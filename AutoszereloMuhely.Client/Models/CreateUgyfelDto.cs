using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Client.Models;

public class CreateUgyfelDto
{
    [Required(ErrorMessage = "A név megadása kötelező.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
    public string Nev { get; set; } = string.Empty;

    [Required(ErrorMessage = "A lakcím megadása kötelező.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
    public string Lakcim { get; set; } = string.Empty;

    [Required(ErrorMessage = "Az email megadása kötelező.")]
    [EmailAddress(ErrorMessage = "Érvényes email címet adjon meg.")]
    public string Email { get; set; } = string.Empty;
}