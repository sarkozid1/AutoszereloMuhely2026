namespace AutoszereloMuhely.Dtos;

// A munka adatait tartalmazó DTO - ezt kapja a kliens válaszként
// Tartalmazza a munkaóra esztimációt is, amit a backend számol
public class MunkaDto
{
    public string Id { get; set; } = string.Empty;
    public string UgyfelId { get; set; } = string.Empty;
    public string Rendszam { get; set; } = string.Empty;
    public int GyartasiEv { get; set; }
    public string Kategoria { get; set; } = string.Empty;
    public string HibaLeiras { get; set; } = string.Empty;
    public int HibaSulyossag { get; set; }
    public string Allapot { get; set; } = string.Empty;

    // A backend által számolt érték - nincs az adatbázisban, mindig frissen generálódik
    public double MunkaoraEsztimacio { get; set; }
}