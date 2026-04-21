namespace AutoszereloMuhely.Services
{
    public class MunkaoraService : IMunkaoraService
    {
        public double Szamol(string kategoria, int gyartasiEv, int hibaSulyossag)
        {
            double kategoriaOra = kategoria switch
            {
                "Karosszeria" => 3,
                "Motor" => 8,
                "Futomu" => 6,
                "Fekberendezes" => 4,
                _ => throw new ArgumentException("ismeretlen kategória!")
            };

            int kor = DateTime.Now.Year - gyartasiEv;
            double korSzorzo = kor switch
            {
                <= 6 => 0.5,
                <= 10 => 1.0,
                <= 20 => 1.5,
                _ => 2.0
            };

            double sulyossagSzorzo = hibaSulyossag switch
            {
                <= 2 => 0.2,
                <= 4 => 0.4,
                <= 7 => 0.6,
                <= 9 => 0.8,
                10 => 1.0,
                _ => throw new ArgumentException("Ervenytelen sulyossag")
            };

            return kategoriaOra * korSzorzo * sulyossagSzorzo;


        }
    }
}
