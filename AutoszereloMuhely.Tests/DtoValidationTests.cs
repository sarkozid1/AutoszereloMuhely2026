using System.ComponentModel.DataAnnotations;
using AutoszereloMuhely.Dtos;

namespace AutoszereloMuhely.Tests;

/// <summary>
/// DTO DataAnnotation validációkat tesztelő egységtesztek.
/// A Validator.TryValidateObject segítségével ellenőrzi, hogy a modellek helyes
/// bemenetre érvényesek, hibás bemenetre hibát adnak.
/// </summary>
public class DtoValidationTests
{
    // Segédmetódus: lefuttatja a DataAnnotations validációt és visszaadja az eredményt
    private static bool Validate(object dto, out List<ValidationResult> errors)
    {
        errors = new List<ValidationResult>();
        var ctx = new ValidationContext(dto);
        return Validator.TryValidateObject(dto, ctx, errors, true);
    }

    // ─────────────────────────────────────────────────────
    // CreateMunkaDto tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // Teljesen kitöltött, érvényes CreateMunkaDto sikeresen átmegy a validáción
    public void CreateMunkaDto_ErvenyesAdatok_ValidacioSikeres()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ABC-123",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Nem indul az autó",
            HibaSulyossag = 7
        };
        Assert.True(Validate(dto, out _));
    }

    [Fact]
    // Üres UgyfelId validációs hibát okoz – [Required] szabály
    public void CreateMunkaDto_UresUgyfelId_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "", // hiányzó kötelező mező
            Rendszam = "ABC-123",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Kisbetűs rendszám validációs hibát okoz – regex: ^[A-Z]{3}-\d{3}$
    public void CreateMunkaDto_KisbetusRendszam_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "abc-123", // kisbetű → érvénytelen
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Csak 2 betűs rendszám validációs hibát okoz – 3 nagybetű szükséges
    public void CreateMunkaDto_KetBetuRendszam_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "AB-123", // 2 betű, nem 3 → érvénytelen
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Kötőjel nélküli rendszám validációs hibát okoz
    public void CreateMunkaDto_KotojelnelkuliRendszam_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ABC123", // kötőjel hiányzik
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Érvényes rendszám formátum (XXX-YYY) sikeresen validálódik
    public void CreateMunkaDto_ErvenyesRendszam_ValidacioSikeres()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ZZZ-999", // érvényes formátum
            GyartasiEv = 2010,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        Assert.True(Validate(dto, out _));
    }

    [Fact]
    // 1899-es gyártási év validációs hibát okoz – minimum értéke 1900
    public void CreateMunkaDto_GyartasiEv1899_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ABC-123",
            GyartasiEv = 1899, // < 1900 → érvénytelen
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Pontosan 1900-as gyártási év érvényes – a minimum határon van
    public void CreateMunkaDto_GyartasiEv1900_ValidacioSikeres()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ABC-123",
            GyartasiEv = 1900, // pontosan 1900 → érvényes
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 5
        };
        Assert.True(Validate(dto, out _));
    }

    [Fact]
    // Súlyosság 0 validációs hibát okoz – minimum értéke 1
    public void CreateMunkaDto_Sulyossag0_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ABC-123",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 0 // < 1 → érvénytelen
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Súlyosság 11 validációs hibát okoz – maximum értéke 10
    public void CreateMunkaDto_Sulyossag11_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ABC-123",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "Hiba",
            HibaSulyossag = 11 // > 10 → érvénytelen
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Csak szóközökből álló HibaLeiras validációs hibát okoz – regex: .*\S.*
    public void CreateMunkaDto_WhitespaceHibaLeiras_ValidacioSikertelen()
    {
        var dto = new CreateMunkaDto
        {
            UgyfelId = "507f1f77bcf86cd799439011",
            Rendszam = "ABC-123",
            GyartasiEv = 2015,
            Kategoria = "Motor",
            HibaLeiras = "   ", // csak szóköz → nem tartalmaz nem-whitespace karaktert
            HibaSulyossag = 5
        };
        Assert.False(Validate(dto, out _));
    }

    // ─────────────────────────────────────────────────────
    // CreateUgyfelDto tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // Teljesen kitöltött, érvényes CreateUgyfelDto sikeresen validálódik
    public void CreateUgyfelDto_ErvenyesAdatok_ValidacioSikeres()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "Kovács János",
            Lakcim = "1234 Budapest, Fő utca 1.",
            Email = "kovacs@example.com"
        };
        Assert.True(Validate(dto, out _));
    }

    [Fact]
    // Üres Nev validációs hibát okoz – [Required] szabály
    public void CreateUgyfelDto_UresNev_ValidacioSikertelen()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "", // kötelező mező, hiányzik
            Lakcim = "Budapest",
            Email = "test@test.com"
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Csak szóközökből álló Nev validációs hibát okoz – regex: .*\S.*
    public void CreateUgyfelDto_WhitespaceNev_ValidacioSikertelen()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "   ", // szóköz-only → nem érvényes
            Lakcim = "Budapest",
            Email = "test@test.com"
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Érvénytelen email cím validációs hibát okoz – [EmailAddress] szabály
    public void CreateUgyfelDto_ErvenytelenEmail_ValidacioSikertelen()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "Kovács János",
            Lakcim = "Budapest",
            Email = "nemvalidemail" // @ hiányzik → érvénytelen
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Domain nélküli email cím validációs hibát okoz
    public void CreateUgyfelDto_EmailDomainNelkul_ValidacioSikertelen()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "Kovács",
            Lakcim = "Budapest",
            Email = "valami@" // domain rész hiányzik → érvénytelen
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Érvényes email cím sikeresen validálódik
    public void CreateUgyfelDto_ErvenyesEmail_ValidacioSikeres()
    {
        var dto = new CreateUgyfelDto
        {
            Nev = "Tesztelő Béla",
            Lakcim = "Debrecen",
            Email = "bela.tesztelő@domain.hu"
        };
        Assert.True(Validate(dto, out _));
    }

    // ─────────────────────────────────────────────────────
    // SajatMunkaDto tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // Érvényes SajatMunkaDto validálódik – nincs UgyfelId mező (szerver tölti JWT-ből)
    public void SajatMunkaDto_ErvenyesAdatok_ValidacioSikeres()
    {
        var dto = new SajatMunkaDto
        {
            // UgyfelId szándékosan nincs itt – a controller tölti ki a JWT claimből
            Rendszam = "XYZ-789",
            GyartasiEv = 2018,
            Kategoria = "Futomu",
            HibaLeiras = "Kopott gumiabroncs",
            HibaSulyossag = 4
        };
        Assert.True(Validate(dto, out _));
    }

    [Fact]
    // Helytelen rendszám formátum SajatMunkaDto-ban validációs hibát okoz
    public void SajatMunkaDto_HelytelenRendszam_ValidacioSikertelen()
    {
        var dto = new SajatMunkaDto
        {
            Rendszam = "1AB-123", // szám az elején → érvénytelen
            GyartasiEv = 2018,
            Kategoria = "Futomu",
            HibaLeiras = "Hiba",
            HibaSulyossag = 4
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Súlyosság határértéke: 10 érvényes SajatMunkaDto-ban
    public void SajatMunkaDto_MaxSulyossag10_ValidacioSikeres()
    {
        var dto = new SajatMunkaDto
        {
            Rendszam = "AAA-000",
            GyartasiEv = 2020,
            Kategoria = "Motor",
            HibaLeiras = "Kritikus hiba",
            HibaSulyossag = 10 // maximum – érvényes
        };
        Assert.True(Validate(dto, out _));
    }

    // ─────────────────────────────────────────────────────
    // UpdateMunkaDto tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // Érvényes UpdateMunkaDto sikeresen validálódik
    public void UpdateMunkaDto_ErvenyesAdatok_ValidacioSikeres()
    {
        var dto = new UpdateMunkaDto
        {
            Rendszam = "DEF-456",
            GyartasiEv = 2010,
            Kategoria = "Karosszeria",
            HibaLeiras = "Karosszéria sérülés",
            HibaSulyossag = 6
        };
        Assert.True(Validate(dto, out _));
    }

    [Fact]
    // Érvénytelen rendszám UpdateMunkaDto-ban validációs hibát okoz
    public void UpdateMunkaDto_ErvenytelenRendszam_ValidacioSikertelen()
    {
        var dto = new UpdateMunkaDto
        {
            Rendszam = "ABC123", // kötőjel hiányzik → érvénytelen
            GyartasiEv = 2010,
            Kategoria = "Karosszeria",
            HibaLeiras = "Hiba",
            HibaSulyossag = 6
        };
        Assert.False(Validate(dto, out _));
    }

    [Fact]
    // Üres HibaLeiras UpdateMunkaDto-ban validációs hibát okoz
    public void UpdateMunkaDto_UresHibaLeiras_ValidacioSikertelen()
    {
        var dto = new UpdateMunkaDto
        {
            Rendszam = "DEF-456",
            GyartasiEv = 2010,
            Kategoria = "Karosszeria",
            HibaLeiras = "", // kötelező mező hiányzik
            HibaSulyossag = 6
        };
        Assert.False(Validate(dto, out _));
    }
}
