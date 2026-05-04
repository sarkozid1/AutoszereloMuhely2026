using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Models
{
    public class Felhasznalo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Felhasznalonev { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // PBKDF2 hashelt jelszó - soha nem kerül ki az API-n
        [Required]
        public string JelszoHash { get; set; } = string.Empty;

        public FelhasznaloSzerep Szerep { get; set; } = FelhasznaloSzerep.Felhasznalo;

        [Required]
        public string Nev { get; set; } = string.Empty;

        public string? Telefonszam { get; set; }

        public string? Lakcim { get; set; }

        // Hivatkozás az ügyféladatokra (csak Felhasznalo szerepnél van feltöltve)
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UgyfelId { get; set; }
    }
}
