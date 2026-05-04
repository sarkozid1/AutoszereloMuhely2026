using AutoszereloMuhely.Controllers;
using AutoszereloMuhely.Dtos;
using AutoszereloMuhely.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace AutoszereloMuhely.Tests;

/// <summary>
/// Az UgyfelekController HTTP végpontjait tesztelő egységtesztek.
/// Az IUgyfelService-t NSubstitute mock-kal helyettesítjük.
/// Az összes végpont "Dolgozo,Adminisztrator" szerepkört igényel,
/// a törlés csak "Adminisztrator" számára elérhető.
/// </summary>
public class UgyfelekControllerTests
{
    private readonly IUgyfelService _mockService;
    private readonly UgyfelekController _controller;

    public UgyfelekControllerTests()
    {
        _mockService = Substitute.For<IUgyfelService>();
        _controller = new UgyfelekController(_mockService);
    }

    // ─────────────────────────────────────────────────────
    // GET /api/ugyfelek – összes ügyfél listázása
    // ─────────────────────────────────────────────────────

    [Fact]
    // GetAll 200-as státusszal adja vissza az ügyfelek listáját
    public async Task GetAll_AlapEset_Ok200EsListaVissza()
    {
        var ugyfelek = new List<UgyfelDto>
        {
            new() { Id = "u1", Nev = "Kovács János", Email = "kovacs@test.com" },
            new() { Id = "u2", Nev = "Nagy Éva",    Email = "nagy@test.com"   }
        };
        _mockService.GetAllAsync().Returns(ugyfelek);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<UgyfelDto>>(ok.Value);
        Assert.Equal(2, lista.Count);
    }

    [Fact]
    // GetAll üres adatbázisnál is 200-at ad vissza (nem 404)
    public async Task GetAll_UresAdatbazis_Ok200EsUresListaVissza()
    {
        _mockService.GetAllAsync().Returns(new List<UgyfelDto>());

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var lista = Assert.IsType<List<UgyfelDto>>(ok.Value);
        Assert.Empty(lista);
    }

    // ─────────────────────────────────────────────────────
    // GET /api/ugyfelek/{id} – egy ügyfél lekérése
    // ─────────────────────────────────────────────────────

    [Fact]
    // GetById létező id-re 200-at és a megfelelő DTO-t adja vissza
    public async Task GetById_LetezoId_Ok200EsUgyfelVissza()
    {
        var ugyfel = new UgyfelDto { Id = "u42", Nev = "Tesztelő Gábor", Email = "gabor@test.com" };
        _mockService.GetByIdAsync("u42").Returns(ugyfel);

        var result = await _controller.GetById("u42");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<UgyfelDto>(ok.Value);
        Assert.Equal("u42", dto.Id);
        Assert.Equal("Tesztelő Gábor", dto.Nev);
    }

    [Fact]
    // GetById nem létező id-re 404 NotFound-ot ad vissza
    public async Task GetById_NemLetezoId_NotFound404()
    {
        _mockService.GetByIdAsync("nemletezik").Returns((UgyfelDto?)null);

        var result = await _controller.GetById("nemletezik");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // ─────────────────────────────────────────────────────
    // POST /api/ugyfelek – új ügyfél létrehozása
    // ─────────────────────────────────────────────────────

    [Fact]
    // Create érvényes DTO-val 201 Created-et ad vissza az új ügyféllel
    public async Task Create_ErvenyesDto_Created201EsUjUgyfelVissza()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "Új Ügyfél",
            Lakcim = "Budapest",
            Email = "uj@test.com"
        };
        var letrehozott = new UgyfelDto { Id = "ujU", Nev = "Új Ügyfél", Email = "uj@test.com" };
        _mockService.CreateAsync(dto).Returns(letrehozott);

        var result = await _controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
        var visszakapott = Assert.IsType<UgyfelDto>(created.Value);
        Assert.Equal("ujU", visszakapott.Id);
    }

    // ─────────────────────────────────────────────────────
    // PUT /api/ugyfelek/{id} – ügyfél adatainak módosítása
    // ─────────────────────────────────────────────────────

    [Fact]
    // Update létező ügyféllel 200-at és a frissített DTO-t adja vissza
    public async Task Update_LetezoId_Ok200EsFrissitettUgyfelVissza()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "Módosított Név",
            Lakcim = "Debrecen",
            Email = "modositott@test.com"
        };
        var frissitett = new UgyfelDto { Id = "u1", Nev = "Módosított Név", Email = "modositott@test.com" };
        _mockService.UpdateAsync("u1", dto).Returns(frissitett);

        var result = await _controller.Update("u1", dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var visszakapott = Assert.IsType<UgyfelDto>(ok.Value);
        Assert.Equal("Módosított Név", visszakapott.Nev);
    }

    [Fact]
    // Update nem létező id-re 404 NotFound-ot ad vissza
    public async Task Update_NemLetezoId_NotFound404()
    {
        var dto = new CreateUgyfelDto { Nev = "X", Lakcim = "Y", Email = "x@y.com" };
        _mockService.UpdateAsync("nemletezik", dto).Returns((UgyfelDto?)null);

        var result = await _controller.Update("nemletezik", dto);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // ─────────────────────────────────────────────────────
    // DELETE /api/ugyfelek/{id} – ügyfél törlése (csak admin, cascade)
    // ─────────────────────────────────────────────────────

    [Fact]
    // Delete létező ügyféllel 204 NoContent-et ad vissza
    public async Task Delete_LetezoId_NoContent204()
    {
        // A service true-t ad vissza, ha sikeres volt a törlés
        _mockService.DeleteAsync("torlendo").Returns(true);

        var result = await _controller.Delete("torlendo");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    // Delete nem létező ügyféllel 404 NotFound-ot ad vissza
    public async Task Delete_NemLetezoId_NotFound404()
    {
        _mockService.DeleteAsync("nemletezik").Returns(false);

        var result = await _controller.Delete("nemletezik");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Delete meghívja a service-t a helyes id-del
    public async Task Delete_LetezoId_ServicetHivjaHellyesIdvel()
    {
        _mockService.DeleteAsync("u77").Returns(true);

        await _controller.Delete("u77");

        // Ellenőrizzük, hogy pontosan egyszer lett meghívva a helyes id-del
        await _mockService.Received(1).DeleteAsync("u77");
    }

    [Fact]
    // Create visszaadott DTO email mezője egyezik a bemenettel
    public async Task Create_ErvenyesDto_EmailMegegyezikABemenettel()
    {
        var dto = new CreateUgyfelDto { Nev = "Email Teszt", Lakcim = "Budapest", Email = "email@check.com" };
        var letrehozott = new UgyfelDto { Id = "e1", Nev = "Email Teszt", Email = "email@check.com" };
        _mockService.CreateAsync(dto).Returns(letrehozott);

        var result = await _controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var visszakapott = Assert.IsType<UgyfelDto>(created.Value);
        Assert.Equal("email@check.com", visszakapott.Email);
    }
}
