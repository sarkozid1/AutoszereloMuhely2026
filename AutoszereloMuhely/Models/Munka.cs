using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Models
{
    public class Munka
    {
        public int Id { get; set; }

        [Required]
        public int UgyfelId { get; set; }
        public Ugyfel? Ugyfel { get; set; }

        [Required(ErrorMessage = "A rendszám megadása kötelező.")]
        [RegularExpression(@"^[A-Z]{3}-\d{3}$",
        ErrorMessage = "A rendszám formátuma: XXX-YYY (X: nagybetű, Y: szám)")]
        public string Rendszam { get; set; } = string.Empty;

        [Required]
        [Range(1900, int.MaxValue, ErrorMessage = "A gyártási év nem lehet kisebb 1900-nál.")]
        public int GyartasiEv { get; set; }

        [Required]
        public MunkaKategoria Kategoria { get; set; }

        [Required(ErrorMessage = "A hiba leírása kötelező.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
        public string HibaLeiras { get; set; } = string.Empty;

        [Required]
        [Range(1, 10, ErrorMessage = "A súlyosság 1 és 10 között legyen.")]
        public int HibaSulyossag { get; set; }

        [Required]
        public MunkaAllapot Allapot { get; set; } = MunkaAllapot.FelvettMunka;

    }
}
