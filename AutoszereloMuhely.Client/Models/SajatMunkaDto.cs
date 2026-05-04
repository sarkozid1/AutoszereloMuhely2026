using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Client.Models;

// Bejelentkezett Felhasznalo által leadott megrendelés - UgyfelId-t a szerver tölti ki a JWT-ből
public class SajatMunkaDto
{
    [Required(ErrorMessage = "A rendszám megadása kötelező.")]
    [RegularExpression(@"^[A-Z]{3}-[0-9]{3}$",
        ErrorMessage = "Formátum: XXX-YYY (3 nagybetű, kötőjel, 3 szám)")]
    public string Rendszam { get; set; } = string.Empty;

    [Required(ErrorMessage = "A gyártási év megadása kötelező.")]
    [Range(1900, 2100, ErrorMessage = "Nem lehet kisebb 1900-nál.")]
    public int GyartasiEv { get; set; }

    [Required(ErrorMessage = "A kategória kiválasztása kötelező.")]
    public string Kategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A hiba leírása kötelező.")]
    public string HibaLeiras { get; set; } = string.Empty;

    [Required(ErrorMessage = "A súlyosság megadása kötelező.")]
    [Range(1, 10, ErrorMessage = "1 és 10 között legyen.")]
    public int HibaSulyossag { get; set; }
}
