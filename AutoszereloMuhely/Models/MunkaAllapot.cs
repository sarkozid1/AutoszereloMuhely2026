namespace AutoszereloMuhely.Models
{
    // A munka állapotát reprezentáló enum
    // A számértékek (0, 1, 2) biztosítják, hogy csak előre lehessen léptetni
    public enum MunkaAllapot
    {
        FelvettMunka = 0,    // Újonnan rögzített munka
        ElvegzesAlatt = 1,   // Szerelés folyamatban
        Befejezett = 2       // Elkészült munka
    }
}