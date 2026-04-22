using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Models
{
    // Az ügyfél (megrendelő) entitás - ezt tárolja az adatbázis
    public class Ugyfel
    {
        // Egyedi azonosító, EF automatikusan generálja (Primary Key)
        public int Id { get; set; }

        // Ügyfél neve - kötelező, nem lehet üres vagy csak szóköz
        [Required(ErrorMessage = "A név megadása kötelező!")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
        public string Nev { get; set; } = string.Empty;

        // Lakcím - kötelező, nem lehet üres vagy csak szóköz
        [Required(ErrorMessage = "A lakcím megadása kötelező.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace.")]
        public string Lakcim { get; set; } = string.Empty;

        // Email cím - kötelező, email formátum ellenőrzéssel
        [Required(ErrorMessage = "Az email megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Érvényes email címet adjon meg.")]
        public string Email { get; set; } = string.Empty;

        // Navigációs property - egy ügyfélhez több munka tartozhat (1:N kapcsolat)
        public ICollection<Munka> Munkak { get; set; } = new List<Munka>();
    }
}