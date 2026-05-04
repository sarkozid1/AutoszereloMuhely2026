namespace AutoszereloMuhely.Client.Models;

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public FelhasznaloDto Felhasznalo { get; set; } = new();
}
