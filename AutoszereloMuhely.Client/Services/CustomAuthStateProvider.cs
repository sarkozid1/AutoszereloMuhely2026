using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace AutoszereloMuhely.Client.Services;

// Egyedi auth state provider - localStorage-ból olvassa a JWT tokent
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private const string TokenKey = "auth_token";

    public CustomAuthStateProvider(IJSRuntime js)
    {
        _js = js;
    }

    private const string LastActivityKey = "last_activity";
    private const int InactivityMinutes = 30;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = ParseClaimsFromJwt(token).ToList();

            // JWT lejárat ellenőrzése
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim != null && long.TryParse(expClaim.Value, out var expSeconds))
            {
                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expSeconds)
                {
                    await ClearSessionAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
            }

            // Inaktivitás ellenőrzése (30 perc)
            var lastActivityStr = await _js.InvokeAsync<string?>("localStorage.getItem", LastActivityKey);
            if (!string.IsNullOrEmpty(lastActivityStr) && long.TryParse(lastActivityStr, out var lastActivity))
            {
                var inactiveSince = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - lastActivity;
                if (inactiveSince > InactivityMinutes * 60)
                {
                    await ClearSessionAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
            }

            // Aktivitás frissítése
            await _js.InvokeVoidAsync("localStorage.setItem", LastActivityKey,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    private async Task ClearSessionAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", "auth_user");
        await _js.InvokeVoidAsync("localStorage.removeItem", LastActivityKey);
    }

    // Értesíti a Blazor komponenseket az auth állapot változásáról
    public void NotifyAuthStateChanged()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    // JWT payload dekódolása (base64url → JSON → claims)
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        // Base64url padding kiegészítése
        var mod = payload.Length % 4;
        if (mod == 2) payload += "==";
        else if (mod == 3) payload += "=";
        payload = payload.Replace('-', '+').Replace('_', '/');

        var jsonBytes = Convert.FromBase64String(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);
        if (keyValuePairs == null) return claims;

        foreach (var kvp in keyValuePairs)
        {
            // A "role" claim speciális kezelése
            if (kvp.Key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" ||
                kvp.Key == "role")
            {
                claims.Add(new Claim(ClaimTypes.Role, kvp.Value.GetString() ?? string.Empty));
            }
            else
            {
                claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
            }
        }

        return claims;
    }
}
