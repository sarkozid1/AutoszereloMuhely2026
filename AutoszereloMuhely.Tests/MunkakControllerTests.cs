using System.Security.Claims;
using AutoszereloMuhely.Controllers;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace AutoszereloMuhely.Tests;

/// <summary>
/// A MunkakController HTTP végpontjait tesztelő egységtesztek.
/// Az IMunkaService-t NSubstitute mock-kal helyettesítjük, így nem szükséges
/// sem adatbázis, sem élő HTTP kiszolgáló a tesztek futtatásához.
/// </summary>
public class MunkakControllerTests
{
    private readonly IMunkaService _mockService;
    private readonly MunkakController _controller;

    public MunkakControllerTests()
    {
        _mockService = Substitute.For<IMunkaService>();
        // MongoDbContext-et null-ként adjuk, mert az itt tesztelt végpontok
        // nem hívják közvetlenül a _context mezőt (csak AnonMegrendeles teszi)
        _controller = new MunkakController(_mockService, null!);
    }

    // Segédmetódus: bejelentkezett felhasználó HTTP kontextusát szimulálja
    private void SetUserClaims(string? ugyfelId = null, string role = "Felhasznalo")
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, role) };
        if (ugyfelId != null)
            claims.Add(new Claim("ugyfelid", ugyfelId));

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    // ─────────────────────────────────────────────────────
    // GET /api/munkak – összes munka listázása
    // ─────────────────────────────────────────────────────

    [Fact]
    // GetAll 200-as státusszal adja vissza a munkák listáját
    public async Task GetAll_AlapEset_Ok200EsListaVissza()
    {
        var munkak = new List<MunkaDto>
        {
            new() { Id = "1", Rendszam = "ABC-123", Allapot = "FelvettMunka" },
            new() { Id = "2", Rendszam = "XYZ-789", Allapot = "Befejezett" }
        };
        _mockService.GetAllAsync().Returns(munkak);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<MunkaDto>>(ok.Value);
        Assert.Equal(2, lista.Count);
    }

    [Fact]
    // GetAll üres lista esetén is 200-at ad vissza (nem 404)
    public async Task GetAll_UresAdatbazis_Ok200EsUresListaVissza()
    {
        _mockService.GetAllAsync().Returns(new List<MunkaDto>());

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<MunkaDto>>(ok.Value);
        Assert.Empty(lista);
    }

    // ─────────────────────────────────────────────────────
    // GET /api/munkak/{id} – egy munka lekérése id alapján
    // ─────────────────────────────────────────────────────

    [Fact]
    // GetById létező id-re 200-at és a megfelelő DTO-t adja vissza
    public async Task GetById_LetezoId_Ok200EsMunkaVissza()
    {
        var munka = new MunkaDto { Id = "abc123", Rendszam = "ABC-123" };
        _mockService.GetByIdAsync("abc123").Returns(munka);

        var result = await _controller.GetById("abc123");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<MunkaDto>(ok.Value);
        Assert.Equal("abc123", dto.Id);
        Assert.Equal("ABC-123", dto.Rendszam);
    }

    [Fact]
    // GetById nem létező id-re 404 NotFound-ot ad vissza
    public async Task GetById_NemLetezoId_NotFound404()
    {
        _mockService.GetByIdAsync("nemletezik").Returns((MunkaDto?)null);

        var result = await _controller.GetById("nemletezik");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // ─────────────────────────────────────────────────────
    // GET /api/munkak/ugyfel/{ugyfelId} – ügyfél munkáinak listázása
    // ─────────────────────────────────────────────────────

    [Fact]
    // GetByUgyfel létező ügyféllel 200-at és a munkák listáját adja vissza
    public async Task GetByUgyfel_LetezoUgyfelId_Ok200EsMunkakVissza()
    {
        var munkak = new List<MunkaDto>
        {
            new() { Id = "m1", UgyfelId = "u1", Rendszam = "TTT-001" },
            new() { Id = "m2", UgyfelId = "u1", Rendszam = "TTT-002" }
        };
        _mockService.GetByUgyfelIdAsync("u1").Returns(munkak);

        var result = await _controller.GetByUgyfel("u1");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<MunkaDto>>(ok.Value);
        Assert.Equal(2, lista.Count);
        Assert.All(lista, m => Assert.Equal("u1", m.UgyfelId));
    }

    [Fact]
    // GetByUgyfel munkák nélküli ügyféllel is 200-at és üres listát ad vissza
    public async Task GetByUgyfel_UgyfelMunkaNelkul_Ok200EsUresListaVissza()
    {
        _mockService.GetByUgyfelIdAsync("u99").Returns(new List<MunkaDto>());

        var result = await _controller.GetByUgyfel("u99");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<MunkaDto>>(ok.Value);
        Assert.Empty(lista);
    }

    // ─────────────────────────────────────────────────────
    // POST /api/munkak – új munka létrehozása (dolgozó/admin)
    // ─────────────────────────────────────────────────────

    [Fact]
    // Create érvényes DTO-val 201 Created-et ad vissza az új munkával
    public async Task Create_ErvenyesDto_Created201EsUjMunkaVissza()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "u1",
            Rendszam = "ABC-123",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba leírása",
            HibaSulyossag = 5
        };
        var letrehozott = new MunkaDto { Id = "uj1", Rendszam = "ABC-123" };
        _mockService.CreateAsync(dto).Returns(letrehozott);

        var result = await _controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
        var visszakapott = Assert.IsType<MunkaDto>(created.Value);
        Assert.Equal("uj1", visszakapott.Id);
    }

    // ─────────────────────────────────────────────────────
    // PUT /api/munkak/{id} – munka módosítása
    // ─────────────────────────────────────────────────────

    [Fact]
    // Update létező munkánál 200-at és a frissített DTO-t adja vissza
    public async Task Update_LetezoId_Ok200EsFrissitettMunkaVissza()
    {
        var dto = new UpdateMunkaDto
        {
            Rendszam = "DEF-456",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Módosított leírás",
            HibaSulyossag = 6
        };
        var frissitett = new MunkaDto { Id = "m1", Rendszam = "DEF-456" };
        _mockService.UpdateAsync("m1", dto).Returns(frissitett);

        var result = await _controller.Update("m1", dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var visszakapott = Assert.IsType<MunkaDto>(ok.Value);
        Assert.Equal("DEF-456", visszakapott.Rendszam);
    }

    [Fact]
    // Update nem létező id-re 404 NotFound-ot ad vissza
    public async Task Update_NemLetezoId_NotFound404()
    {
        var dto = new UpdateMunkaDto
        {
            Rendszam = "DEF-456",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        _mockService.UpdateAsync("nemletezik", dto).Returns((MunkaDto?)null);

        var result = await _controller.Update("nemletezik", dto);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // ─────────────────────────────────────────────────────
    // PATCH /api/munkak/{id}/allapot – állapot léptetése
    // ─────────────────────────────────────────────────────

    [Fact]
    // UpdateAllapot érvényes átmenetnél 200-at és a frissített állapotot adja vissza
    public async Task UpdateAllapot_ErvenyesAtmenet_Ok200EsFrissitettAllapotVissza()
    {
        var frissitett = new MunkaDto { Id = "m1", Allapot = "ElvegzesAlatt" };
        _mockService.UpdateAllapotAsync("m1", "ElvegzesAlatt").Returns(frissitett);

        var result = await _controller.UpdateAllapot("m1", "ElvegzesAlatt");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<MunkaDto>(ok.Value);
        Assert.Equal("ElvegzesAlatt", dto.Allapot);
    }

    [Fact]
    // UpdateAllapot nem létező munkánál 404 NotFound-ot ad vissza
    public async Task UpdateAllapot_NemLetezoId_NotFound404()
    {
        _mockService.UpdateAllapotAsync("nemletezik", "ElvegzesAlatt").Returns((MunkaDto?)null);

        var result = await _controller.UpdateAllapot("nemletezik", "ElvegzesAlatt");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // ─────────────────────────────────────────────────────
    // DELETE /api/munkak/{id} – munka törlése
    // ─────────────────────────────────────────────────────

    [Fact]
    // Delete létező munkánál 204 NoContent-et ad vissza
    public async Task Delete_LetezoId_NoContent204()
    {
        _mockService.DeleteAsync("torlendo").Returns(true);

        var result = await _controller.Delete("torlendo");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    // Delete nem létező munkánál 404 NotFound-ot ad vissza
    public async Task Delete_NemLetezoId_NotFound404()
    {
        _mockService.DeleteAsync("nemletezik").Returns(false);

        var result = await _controller.Delete("nemletezik");

        Assert.IsType<NotFoundResult>(result);
    }

    // ─────────────────────────────────────────────────────
    // GET /api/munkak/sajat – bejelentkezett felhasználó saját munkái
    // ─────────────────────────────────────────────────────

    [Fact]
    // GetSajat érvényes ugyfelid claim-mel 200-at és a saját munkákat adja vissza
    public async Task GetSajat_ErvenyesUgyfelIdClaim_Ok200EsSajatMunkakVissza()
    {
        SetUserClaims(ugyfelId: "ugyfel42");
        var munkak = new List<MunkaDto>
        {
            new() { Id = "m1", UgyfelId = "ugyfel42", Rendszam = "AAA-001" }
        };
        _mockService.GetByUgyfelIdAsync("ugyfel42").Returns(munkak);

        var result = await _controller.GetSajat();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<MunkaDto>>(ok.Value);
        Assert.Single(lista);
        Assert.Equal("ugyfel42", lista[0].UgyfelId);
    }

    [Fact]
    // GetSajat ugyfelid claim nélkül üres listát ad vissza (nem 401/404)
    public async Task GetSajat_HianyzoUgyfelIdClaim_Ok200EsUresListaVissza()
    {
        // Nincs "ugyfelid" claim a tokenben (pl. hibás regisztráció)
        SetUserClaims(ugyfelId: null);

        var result = await _controller.GetSajat();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<MunkaDto>>(ok.Value);
        Assert.Empty(lista);
    }

    // ─────────────────────────────────────────────────────
    // POST /api/munkak/sajat – bejelentkezett felhasználó megrendelése
    // ─────────────────────────────────────────────────────

    [Fact]
    // CreateSajat érvényes ugyfelid claim-mel 200-at és az új munkát adja vissza
    public async Task CreateSajat_ErvenyesUgyfelIdClaim_Ok200EsUjMunkaVissza()
    {
        SetUserClaims(ugyfelId: "ugyfel99");
        var sajatDto = new SajatMunkaDto
        {
            Rendszam = "GHI-321",
            GyartasiEv = 2019,
            Kategoria = "Futomu",
            HibaLeiras = "Kopott gumiabroncs",
            HibaSulyossag = 3
        };
        // Arg.Any<CreateMunkaDto>() jelzi: bármilyen CreateMunkaDto-t elfogad
        // (a controller maga állítja össze az UgyfelId-del)
        _mockService.CreateAsync(Arg.Any<CreateMunkaDto>()).Returns(new MunkaDto { Id = "uj99", Rendszam = "GHI-321" });

        var result = await _controller.CreateSajat(sajatDto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<MunkaDto>(ok.Value);
        Assert.Equal("uj99", dto.Id);
    }

    [Fact]
    // CreateSajat ugyfelid claim nélkül 400 BadRequest-et ad vissza
    public async Task CreateSajat_HianyzoUgyfelIdClaim_BadRequest400()
    {
        SetUserClaims(ugyfelId: null); // JWT-ben nincs ugyfelid claim
        var dto = new SajatMunkaDto
        {
            Rendszam = "ABC-123",
            GyartasiEv = 2018,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };

        var result = await _controller.CreateSajat(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    // CreateSajat az UgyfelId-et a claim-ből veszi, nem a DTO-ból
    public async Task CreateSajat_UgyfelIdotClaimbolToltiKi_NemDtobol()
    {
        SetUserClaims(ugyfelId: "ugyfelJWT");
        var sajatDto = new SajatMunkaDto
        {
            Rendszam = "JKL-654",
            GyartasiEv = 2020,
            Kategoria = "Karosszeria",
            HibaLeiras = "Karosszéria sérülés",
            HibaSulyossag = 7
        };

        CreateMunkaDto? kaposttDto = null;
        // Elkapjuk a service-nek átadott DTO-t, hogy ellenőrizzük az UgyfelId-et
        _mockService.CreateAsync(Arg.Do<CreateMunkaDto>(d => kaposttDto = d))
                    .Returns(new MunkaDto { Id = "x", UgyfelId = "ugyfelJWT" });

        await _controller.CreateSajat(sajatDto);

        // Ellenőrzés: a service-nek átadott DTO UgyfelId-je a JWT claim-ből jön
        Assert.NotNull(kaposttDto);
        Assert.Equal("ugyfelJWT", kaposttDto!.UgyfelId);
    }
}
