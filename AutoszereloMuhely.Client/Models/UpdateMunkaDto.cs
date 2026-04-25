using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Client.Models;

public class UpdateMunkaDto
{
    [Required(ErrorMessage = "A rendszám megadása kötelező.")]
    [RegularExpression(@"^[A-Z]{3}-[0-9]{3}$",
        ErrorMessage = "Formátum: XXX-YYY (3 nagybetű, kötőjel, 3 szám)")]
    public string Rendszam { get; set; } = string.Empty;

    [Required(ErrorMessage = "A gyártási év megadása kötelező.")]
    [Range(1900, int.MaxValue, ErrorMessage = "Nem lehet kisebb 1900-nál.")]
    public int GyartasiEv { get; set; }

    [Required(ErrorMessage = "A kategória kiválasztása kötelező.")]
    public string Kategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A hiba leírása kötelező.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
    public string HibaLeiras { get; set; } = string.Empty;

    [Required(ErrorMessage = "A súlyosság megadása kötelező.")]
    [Range(1, 10, ErrorMessage = "1 és 10 között legyen.")]
    public int HibaSulyossag { get; set; }
}