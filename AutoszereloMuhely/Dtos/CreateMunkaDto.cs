using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Dtos;

// Új munka létrehozásához szükséges adatok
// Nincs benne Id (adatbázis generálja) és Allapot (automatikusan FelvettMunka)
public class CreateMunkaDto
{
    [Required]
    public string UgyfelId { get; set; } = string.Empty;

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