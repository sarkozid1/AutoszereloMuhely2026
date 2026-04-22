using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Dtos;

// Munka módosításához szükséges adatok
// Nincs benne UgyfelId, mert azt nem lehet utólag változtatni
public class UpdateMunkaDto
{
    [Required]
    [RegularExpression(@"^[A-Z]{3}-\d{3}$")]
    public string Rendszam { get; set; } = string.Empty;

    [Required]
    [Range(1900, int.MaxValue)]
    public int GyartasiEv { get; set; }

    [Required]
    public string Kategoria { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@".*\S.*")]
    public string HibaLeiras { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int HibaSulyossag { get; set; }
}