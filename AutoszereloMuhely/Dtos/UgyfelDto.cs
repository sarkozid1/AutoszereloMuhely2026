namespace AutoszereloMuhely.Dtos;

// Ügyfél adatait tartalmazó DTO - ezt kapja a kliens válaszként
public class UgyfelDto
{
    public string Id { get; set; } = string.Empty;
    public string Nev { get; set; } = string.Empty;
    public string Lakcim { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}