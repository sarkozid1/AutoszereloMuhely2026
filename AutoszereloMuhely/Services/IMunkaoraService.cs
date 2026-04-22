namespace AutoszereloMuhely.Services;

// Munkaóra esztimáció számítás interfész
// Interfészt használunk, hogy a unit tesztekben mockolni lehessen
public interface IMunkaoraService
{
    // Kiszámolja a becsült munkaórákat a kategória, gyártási év és súlyosság alapján
    double Szamol(string kategoria, int gyartasiEv, int hibaSulyossag);
}