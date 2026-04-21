namespace AutoszereloMuhely.Services;

public interface IMunkaoraService
{
    double Szamol(string kategoria, int gyartasiEv, int hibaSulyossag);
}