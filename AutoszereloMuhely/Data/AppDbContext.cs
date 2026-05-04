using MongoDB.Driver;
using AutoszereloMuhely.Models;
using Microsoft.Extensions.Options;

namespace AutoszereloMuhely.Data;

// MongoDB beállítások - az appsettings.json-ból olvassa be
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

// MongoDB adatbázis kontextus - ez a "kapu" a MongoDB-hez
// Minden adatbázis művelet ezen keresztül történik
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    // Konstruktor - a beállításokat az appsettings.json-ból kapja meg
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    // Az Ugyfelek kollekció elérése
    public IMongoCollection<Ugyfel> Ugyfelek => _database.GetCollection<Ugyfel>("ugyfelek");

    // A Munkak kollekció elérése
    public IMongoCollection<Munka> Munkak => _database.GetCollection<Munka>("munkak");

    // A Felhasznalok kollekció elérése (bejelentkezési és szerepkör adatok)
    public IMongoCollection<Felhasznalo> Felhasznalok => _database.GetCollection<Felhasznalo>("felhasznalok");
}
