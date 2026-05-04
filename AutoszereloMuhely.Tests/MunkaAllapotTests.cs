using AutoszereloMuhely.Models;

namespace AutoszereloMuhely.Tests;

/// <summary>
/// A MunkaAllapot enum helyes értékeit és az állapot-átmenet logikáját tesztelő egységtesztek.
/// A service az enum egész számértékein alapuló összehasonlítással valósítja meg
/// a csak-előre léptethetőséget: if ((int)ujAllapot <= (int)jelenlegiAllapot) → kivétel.
/// </summary>
public class MunkaAllapotTests
{
    // ─────────────────────────────────────────────────────
    // Enum értéktesztek – a számértékek kritikusak az összehasonlításhoz
    // ─────────────────────────────────────────────────────

    [Fact]
    // FelvettMunka értéke 0 kell legyen – ez a kezdeti állapot, a legkisebb érték
    public void FelvettMunka_EnumErteke_Nulla()
    {
        Assert.Equal(0, (int)MunkaAllapot.FelvettMunka);
    }

    [Fact]
    // ElvegzesAlatt értéke 1 kell legyen – a közbülső állapot
    public void ElvegzesAlatt_EnumErteke_Egy()
    {
        Assert.Equal(1, (int)MunkaAllapot.ElvegzesAlatt);
    }

    [Fact]
    // Befejezett értéke 2 kell legyen – a végállapot, a legnagyobb érték
    public void Befejezett_EnumErteke_Ketto()
    {
        Assert.Equal(2, (int)MunkaAllapot.Befejezett);
    }

    // ─────────────────────────────────────────────────────
    // Állapot sorrend tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // FelvettMunka < ElvegzesAlatt – a sorrend helyes az összehasonlíthatósághoz
    public void AllapotSorrend_FelvettMunkaKisebbMintElvegzesAlatt()
    {
        Assert.True((int)MunkaAllapot.FelvettMunka < (int)MunkaAllapot.ElvegzesAlatt);
    }

    [Fact]
    // ElvegzesAlatt < Befejezett – a sorrend helyes
    public void AllapotSorrend_ElvegzesAlattKisebbMintBefejezett()
    {
        Assert.True((int)MunkaAllapot.ElvegzesAlatt < (int)MunkaAllapot.Befejezett);
    }

    [Fact]
    // FelvettMunka < Befejezett – a teljes lánc sorrendje helyes
    public void AllapotSorrend_FelvettMunkaKisebbMintBefejezett()
    {
        Assert.True((int)MunkaAllapot.FelvettMunka < (int)MunkaAllapot.Befejezett);
    }

    // ─────────────────────────────────────────────────────
    // Átmenet érvényességi tesztek
    // (a service logikát tükrözik: új > jelenlegi → érvényes)
    // ─────────────────────────────────────────────────────

    [Fact]
    // FelvettMunka → ElvegzesAlatt: érvényes előre lépés
    public void AllapotAtmenet_FelvettbolElvegzesAlatt_ErvenyesEloreLeptes()
    {
        var jelenlegi = MunkaAllapot.FelvettMunka;
        var uj = MunkaAllapot.ElvegzesAlatt;
        // A service: if ((int)uj <= (int)jelenlegi) → kivétel
        // Tehát (int)uj > (int)jelenlegi → érvényes átmenet
        Assert.True((int)uj > (int)jelenlegi);
    }

    [Fact]
    // ElvegzesAlatt → Befejezett: érvényes előre lépés
    public void AllapotAtmenet_ElvegzesAlattbolBefejezett_ErvenyesEloreLeptes()
    {
        var jelenlegi = MunkaAllapot.ElvegzesAlatt;
        var uj = MunkaAllapot.Befejezett;
        Assert.True((int)uj > (int)jelenlegi);
    }

    [Fact]
    // FelvettMunka → Befejezett: érvényes előre lépés (két lépés egyszerre)
    public void AllapotAtmenet_FelvettbolBefejezett_ErvenyesEloreLeptes()
    {
        var jelenlegi = MunkaAllapot.FelvettMunka;
        var uj = MunkaAllapot.Befejezett;
        Assert.True((int)uj > (int)jelenlegi);
    }

    [Fact]
    // ElvegzesAlatt → FelvettMunka: tiltott visszalépés
    public void AllapotAtmenet_ElvegzesAlattbolFelvett_TiltottVisszaLepes()
    {
        var jelenlegi = MunkaAllapot.ElvegzesAlatt;
        var uj = MunkaAllapot.FelvettMunka;
        // (int)uj <= (int)jelenlegi → a service kivételt dobna
        Assert.False((int)uj > (int)jelenlegi);
    }

    [Fact]
    // Befejezett → ElvegzesAlatt: tiltott visszalépés
    public void AllapotAtmenet_BefejezetbolElvegzesAlatt_TiltottVisszaLepes()
    {
        var jelenlegi = MunkaAllapot.Befejezett;
        var uj = MunkaAllapot.ElvegzesAlatt;
        Assert.False((int)uj > (int)jelenlegi);
    }

    [Fact]
    // Azonos állapotra lépés tiltott (nincs változás)
    public void AllapotAtmenet_AzonosaAllapotra_TiltottAtmenet()
    {
        var jelenlegi = MunkaAllapot.ElvegzesAlatt;
        var uj = MunkaAllapot.ElvegzesAlatt; // ugyanaz → nem megengedett
        Assert.False((int)uj > (int)jelenlegi);
    }

    // ─────────────────────────────────────────────────────
    // String parse tesztek – a controller stringként kapja az állapotot
    // ─────────────────────────────────────────────────────

    [Fact]
    // "FelvettMunka" string → FelvettMunka enum (a service Enum.Parse-t használ)
    public void AllapotParse_FelvettMunkaString_HellyesenParsol()
    {
        var result = Enum.Parse<MunkaAllapot>("FelvettMunka");
        Assert.Equal(MunkaAllapot.FelvettMunka, result);
    }

    [Fact]
    // "ElvegzesAlatt" string → ElvegzesAlatt enum
    public void AllapotParse_ElvegzesAlattString_HellyesenParsol()
    {
        var result = Enum.Parse<MunkaAllapot>("ElvegzesAlatt");
        Assert.Equal(MunkaAllapot.ElvegzesAlatt, result);
    }

    [Fact]
    // "Befejezett" string → Befejezett enum
    public void AllapotParse_BefejezetString_HellyesenParsol()
    {
        var result = Enum.Parse<MunkaAllapot>("Befejezett");
        Assert.Equal(MunkaAllapot.Befejezett, result);
    }

    [Fact]
    // Érvénytelen string parse ArgumentException-t dob
    public void AllapotParse_ErvenytelenString_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Enum.Parse<MunkaAllapot>("NemLetezik"));
    }
}
