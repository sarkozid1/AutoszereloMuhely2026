using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "A felhasználónév megadása kötelező.")]
    public string Felhasznalonev { get; set; } = string.Empty;

    [Required(ErrorMessage = "A jelszó megadása kötelező.")]
    public string Jelszo { get; set; } = string.Empty;
}
