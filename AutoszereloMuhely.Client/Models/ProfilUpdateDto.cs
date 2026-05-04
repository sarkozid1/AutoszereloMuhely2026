using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Client.Models;

public class ProfilUpdateDto
{
    [Required(ErrorMessage = "A név megadása kötelező.")]
    public string Nev { get; set; } = string.Empty;

    public string? Telefonszam { get; set; }

    public string? Lakcim { get; set; }
}
