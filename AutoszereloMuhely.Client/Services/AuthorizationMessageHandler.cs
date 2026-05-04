using Microsoft.JSInterop;

namespace AutoszereloMuhely.Client.Services;

// Minden kimenő API kéréshez automatikusan hozzáadja a JWT tokent
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;
    private const string TokenKey = "auth_token";

    public AuthorizationMessageHandler(IJSRuntime js)
    {
        _js = js;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        catch { /* JSInterop előtt nem érhető el - prerendering esetén */ }

        return await base.SendAsync(request, cancellationToken);
    }
}
