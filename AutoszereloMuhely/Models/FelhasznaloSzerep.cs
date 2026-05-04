namespace AutoszereloMuhely.Models
{
    public enum FelhasznaloSzerep
    {
        Felhasznalo = 0,      // Normál regisztrált felhasználó (megrendelő)
        Dolgozo = 1,          // Irodai dolgozó (szerelők, ügyintézők)
        Adminisztrator = 2    // Rendszergazda - teljes hozzáférés
    }
}
