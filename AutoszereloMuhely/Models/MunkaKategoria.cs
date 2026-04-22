namespace AutoszereloMuhely.Models
{
    // A munka típusát meghatározó enum
    // Minden kategóriához más alap munkaóra tartozik az esztimációnál
    public enum MunkaKategoria
    {
        Karosszeria,    // 3 óra alap
        Motor,          // 8 óra alap
        Futomu,         // 6 óra alap
        Fekberendezes   // 4 óra alap
    }
}