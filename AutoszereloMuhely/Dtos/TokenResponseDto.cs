namespace AutoszereloMuhely.Dtos;

// JWT token válasz bejelentkezéskor
public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public FelhasznaloDto Felhasznalo { get; set; } = new();
}
