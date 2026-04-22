namespace AutoszereloMuhely.Client.Models;

public class MunkaDto
{
    public int Id { get; set; }
    public int UgyfelId { get; set; }
    public string Rendszam { get; set; } = string.Empty;
    public int GyartasiEv { get; set; }
    public string Kategoria { get; set; } = string.Empty;
    public string HibaLeiras { get; set; } = string.Empty;
    public int HibaSulyossag { get; set; }
    public string Allapot { get; set; } = string.Empty;
    public double MunkaoraEsztimacio { get; set; }
}