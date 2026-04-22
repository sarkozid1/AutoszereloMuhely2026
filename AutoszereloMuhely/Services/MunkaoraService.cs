namespace AutoszereloMuhely.Services
{
    // Munkaóra esztimáció számítás implementáció
    // Képlet: kategória alap óra × kor szorzó × súlyosság szorzó
    public class MunkaoraService : IMunkaoraService
    {
        public double Szamol(string kategoria, int gyartasiEv, int hibaSulyossag)
        {
            // 1. lépés: alap munkaóra a kategória alapján
            double kategoriaOra = kategoria switch
            {
                "Karosszeria" => 3,
                "Motor" => 8,
                "Futomu" => 6,
                "Fekberendezes" => 4,
                _ => throw new ArgumentException("Ismeretlen kategória!")
            };

            // 2. lépés: kor szorzó - minél idősebb az autó, annál több munka
            int kor = DateTime.Now.Year - gyartasiEv;
            double korSzorzo = kor switch
            {
                <= 5 => 0.5,   // 0-5 éves autó
                <= 10 => 1.0,  // 5-10 éves autó
                <= 20 => 1.5,  // 10-20 éves autó
                _ => 2.0       // 20+ éves autó
            };

            // 3. lépés: súlyosság szorzó - minél súlyosabb a hiba, annál több munka
            double sulyossagSzorzo = hibaSulyossag switch
            {
                <= 2 => 0.2,   // enyhe hiba
                <= 4 => 0.4,
                <= 7 => 0.6,
                <= 9 => 0.8,
                10 => 1.0,     // legsúlyosabb hiba
                _ => throw new ArgumentException("Érvénytelen súlyosság")
            };

            // Végső számítás: a három érték szorzata adja az esztimált munkaórákat
            return kategoriaOra * korSzorzo * sulyossagSzorzo;
        }
    }
}