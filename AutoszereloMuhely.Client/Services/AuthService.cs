using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using AutoszereloMuhely.Client.Models;

namespace AutoszereloMuhely.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    private const string TokenKey = "auth_token";
    private const string UserKey = "auth_user";

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(LoginDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/bejelentkezes", dto);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadFromJsonAsync<JsonElement>();
            return (false, err.TryGetProperty("message", out var msg) ? msg.GetString() : "Hiba történt.");
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        if (result == null) return (false, "Érvénytelen válasz.");

        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
        await _js.InvokeVoidAsync("localStorage.setItem", UserKey, JsonSerializer.Serialize(result.Felhasznalo));
        await _js.InvokeVoidAsync("localStorage.setItem", "last_activity",
            DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/regisztracio", dto);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadFromJsonAsync<JsonElement>();
            return (false, err.TryGetProperty("message", out var msg) ? msg.GetString() : "Hiba történt.");
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        if (result == null) return (false, "Érvénytelen válasz.");

        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
        await _js.InvokeVoidAsync("localStorage.setItem", UserKey, JsonSerializer.Serialize(result.Felhasznalo));
        await _js.InvokeVoidAsync("localStorage.setItem", "last_activity",
            DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", UserKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", "last_activity");
    }

    public async Task<string?> GetTokenAsync()
        => await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    public async Task<FelhasznaloDto?> GetCurrentUserAsync()
    {
        var json = await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);
        if (string.IsNullOrEmpty(json)) return null;
        return JsonSerializer.Deserialize<FelhasznaloDto>(json);
    }
}
