using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Models
{
    // Dolgozó (munkavállaló) adatait reprezentáló model osztály
    public class Dolgozo
    {
        // MongoDB dokumentum azonosítója - ObjectId formátumban tárolva
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        // Dolgozó teljes neve - kötelező, nem lehet csak whitespace
        [Required(ErrorMessage = "A név megadása kötelező!")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace!")]
        public string Nev { get; set; } = string.Empty;

        // Bejelentkezéshez használt felhasználónév - kötelező, nem lehet csak whitespace
        [Required(ErrorMessage = "A felhasználónév megadása kötelező!")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace!")]
        public string Felhasznalonev { get; set; } = string.Empty;

        // Bejelentkezéshez használt jelszó - kötelező, nem lehet csak whitespace
        [Required(ErrorMessage = "A jelszó megadása kötelező!")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nem lehet csak whitespace!")]
        public string Jelszo { get; set; } = string.Empty;
    }
}
