using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Models
{
    // A szerelési munka entitás - ezt tárolja az adatbázis
    public class Munka
    {
        // Egyedi azonosító, EF automatikusan generálja (Primary Key)
        public int Id { get; set; }

        // Idegen kulcs (Foreign Key) - melyik ügyfélhez tartozik ez a munka
        [Required]
        public int UgyfelId { get; set; }

        // Navigációs property - az ügyfél objektum, akit az UgyfelId hivatkozik
        public Ugyfel? Ugyfel { get; set; }

        // Rendszám - kötelező, formátum: XXX-YYY (3 nagybetű, kötőjel, 3 szám)
        [Required(ErrorMessage = "A rendszám megadása kötelező.")]
        [RegularExpression(@"^[A-Z]{3}-\d{3}$",
            ErrorMessage = "A rendszám formátuma: XXX-YYY (X: nagybetű, Y: szám)")]
        public string Rendszam { get; set; } = string.Empty;

        // Gyártási év - nem lehet kisebb 1900-nál
        [Required]
        [Range(1900, int.MaxValue, ErrorMessage = "A gyártási év nem lehet kisebb 1900-nál.")]
        public int GyartasiEv { get; set; }

        // Munka kategóriája - enum típus, csak a 4 megadott érték lehet
        [Required]
        public MunkaKategoria Kategoria { get; set; }

        // A hiba szöveges leírása - kötelező, nem lehet üres
        [Required(ErrorMessage = "A hiba leírása kötelező.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
        public string HibaLeiras { get; set; } = string.Empty;

        // Hiba súlyossága 1-10 skálán - az esztimáció számításhoz kell
        [Required]
        [Range(1, 10, ErrorMessage = "A súlyosság 1 és 10 között legyen.")]
        public int HibaSulyossag { get; set; }

        // Munka állapota - alapértelmezetten "FelvettMunka", csak előre léptethető
        [Required]
        public MunkaAllapot Allapot { get; set; } = MunkaAllapot.FelvettMunka;
    }
}