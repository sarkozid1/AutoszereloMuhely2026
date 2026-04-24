using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Models
{
    // Az ügyfél (megrendelő) entitás - ezt tárolja a MongoDB adatbázis
    public class Ugyfel
    {
        // Egyedi azonosító - MongoDB ObjectId stringként tárolva
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

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
    }
}