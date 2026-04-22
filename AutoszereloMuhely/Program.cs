using AutoszereloMuhely.Data;
using AutoszereloMuhely.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Service-ek regisztrálása (Dependency Injection) ---

// Controllerek regisztrálása - az ASP.NET ezáltal tudja, hová irányítsa a HTTP kéréseket
builder.Services.AddControllers();

// Swagger UI engedélyezése - interaktív API tesztelő felület (csak fejlesztéskor)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework beállítása SQLite adatbázissal
// Az autoszerelo.db fájl a projekt mappájában jön létre
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=autoszerelo.db"));

// Saját service-ek regisztrálása
// AddScoped = minden HTTP kérésnél új példány jön létre
builder.Services.AddScoped<IMunkaoraService, MunkaoraService>();
builder.Services.AddScoped<IMunkaService, MunkaService>();
builder.Services.AddScoped<IUgyfelService, UgyfelService>();

// CORS engedélyezése - a Blazor kliens más porton fut, enélkül a böngésző blokkolná a kéréseket
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(b =>
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// --- HTTP kérés feldolgozási pipeline ---

// Swagger UI csak fejlesztési módban érhető el
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS middleware - engedélyezi a cross-origin kéréseket
app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();

// Controller végpontok leképezése az URL-ekre
app.MapControllers();

app.Run();