using AutoszereloMuhely.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// Blazor WebAssembly alkalmazás belépési pontja
// Ez a kód a böngészőben fut, nem a szerveren
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Az App komponens lesz a gyökér - az index.html-ben az #app elembe renderelődik
builder.RootComponents.Add<App>("#app");

// A <head> tag végére kerülő elemek kezelése (pl. title, meta tagek)
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient regisztrálása - ezzel kommunikál a kliens az API-val
// A BaseAddress az API szerver címe (a launchSettings.json-ból származó port)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5086")
});

// Alkalmazás indítása
await builder.Build().RunAsync();