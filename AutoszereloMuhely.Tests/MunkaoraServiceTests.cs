using AutoszereloMuhely.Services;

namespace AutoszereloMuhely.Tests;

/// <summary>
/// A MunkaoraService.Szamol() metódusát tesztelő egységtesztek.
/// Az osztálynak nincsenek külső függőségei (pure function), ezért mock nélkül tesztelhető.
/// Képlet: kategoriaAlaporak × korSzorzó × súlyossagSzorzó
/// </summary>
public class MunkaoraServiceTests
{
    private readonly MunkaoraService _service = new();

    // ─────────────────────────────────────────────────────
    // Kategória alap-óra tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // Karosszéria kategória alap-órája 3 – ellenőrzés új autóval (kor szorzó 0.5) és enyhe hibával (szorzó 0.2)
    public void Szamol_Karosszeria_AlaporaHarom()
    {
        int ev = DateTime.Now.Year - 3; // 3 éves autó → kor szorzó 0.5
        double result = _service.Szamol("Karosszeria", ev, 1); // súlyosság 1 → szorzó 0.2
        // 3 × 0.5 × 0.2 = 0.3
        Assert.Equal(0.3, result, 10);
    }

    [Fact]
    // Motor kategória alap-órája 8 – ellenőrzés új autóval és enyhe hibával
    public void Szamol_Motor_AlaporaNyolc()
    {
        int ev = DateTime.Now.Year - 3; // kor szorzó 0.5
        double result = _service.Szamol("Motor", ev, 1); // szorzó 0.2
        // 8 × 0.5 × 0.2 = 0.8
        Assert.Equal(0.8, result, 10);
    }

    [Fact]
    // Futómű kategória alap-órája 6 – ellenőrzés új autóval és enyhe hibával
    public void Szamol_Futomu_AlaporaHat()
    {
        int ev = DateTime.Now.Year - 3; // kor szorzó 0.5
        double result = _service.Szamol("Futomu", ev, 1); // szorzó 0.2
        // 6 × 0.5 × 0.2 = 0.6
        Assert.Equal(0.6, result, 10);
    }

    [Fact]
    // Fékberendezés kategória alap-órája 4 – ellenőrzés új autóval és enyhe hibával
    public void Szamol_Fekberendezes_AlaporaNegy()
    {
        int ev = DateTime.Now.Year - 3; // kor szorzó 0.5
        double result = _service.Szamol("Fekberendezes", ev, 1); // szorzó 0.2
        // 4 × 0.5 × 0.2 = 0.4
        Assert.Equal(0.4, result, 10);
    }

    // ─────────────────────────────────────────────────────
    // Kor szorzó tesztek (különböző korosztályok)
    // ─────────────────────────────────────────────────────

    [Fact]
    // 0–5 éves autóra a kor szorzó 0.5 – Motor kategóriával, max súlyossággal tesztelve
    public void Szamol_KorSzorzo_UjAutoFelSzorzo()
    {
        int ev = DateTime.Now.Year - 3; // 3 éves → kor szorzó 0.5
        double result = _service.Szamol("Motor", ev, 10); // súlyosság 10 → szorzó 1.0
        // 8 × 0.5 × 1.0 = 4.0
        Assert.Equal(4.0, result, 10);
    }

    [Fact]
    // 6–10 éves autóra a kor szorzó 1.0 – Motor kategóriával, max súlyossággal tesztelve
    public void Szamol_KorSzorzo_KozepesAutoEgyesSzorzo()
    {
        int ev = DateTime.Now.Year - 7; // 7 éves → kor szorzó 1.0
        double result = _service.Szamol("Motor", ev, 10); // szorzó 1.0
        // 8 × 1.0 × 1.0 = 8.0
        Assert.Equal(8.0, result, 10);
    }

    [Fact]
    // 11–20 éves autóra a kor szorzó 1.5 – Motor kategóriával, max súlyossággal tesztelve
    public void Szamol_KorSzorzo_RegebbiAutoEgyfelesSzorzo()
    {
        int ev = DateTime.Now.Year - 15; // 15 éves → kor szorzó 1.5
        double result = _service.Szamol("Motor", ev, 10); // szorzó 1.0
        // 8 × 1.5 × 1.0 = 12.0
        Assert.Equal(12.0, result, 10);
    }

    [Fact]
    // 20+ éves autóra a kor szorzó 2.0 – Motor kategóriával, max súlyossággal tesztelve
    public void Szamol_KorSzorzo_OregAutoKettesSzorzo()
    {
        int ev = DateTime.Now.Year - 25; // 25 éves → kor szorzó 2.0
        double result = _service.Szamol("Motor", ev, 10); // szorzó 1.0
        // 8 × 2.0 × 1.0 = 16.0
        Assert.Equal(16.0, result, 10);
    }

    // ─────────────────────────────────────────────────────
    // Kor szorzó határérték tesztek (pontosan a határon)
    // ─────────────────────────────────────────────────────

    [Fact]
    // Pontosan 5 éves autó: kor == 5, ami ≤ 5 → szorzó 0.5 (nem 1.0)
    public void Szamol_KorSzorzo_Pontosan5EvesAutoFelSzorzo()
    {
        int ev = DateTime.Now.Year - 5; // pontosan 5 éves → 0.5
        double result = _service.Szamol("Motor", ev, 10);
        // 8 × 0.5 × 1.0 = 4.0
        Assert.Equal(4.0, result, 10);
    }

    [Fact]
    // Pontosan 10 éves autó: kor == 10, ami ≤ 10 → szorzó 1.0 (nem 1.5)
    public void Szamol_KorSzorzo_Pontosan10EvesAutoEgyesSzorzo()
    {
        int ev = DateTime.Now.Year - 10; // pontosan 10 éves → 1.0
        double result = _service.Szamol("Motor", ev, 10);
        // 8 × 1.0 × 1.0 = 8.0
        Assert.Equal(8.0, result, 10);
    }

    [Fact]
    // Pontosan 20 éves autó: kor == 20, ami ≤ 20 → szorzó 1.5 (nem 2.0)
    public void Szamol_KorSzorzo_Pontosan20EvesAutoEgyfelesSzorzo()
    {
        int ev = DateTime.Now.Year - 20; // pontosan 20 éves → 1.5
        double result = _service.Szamol("Motor", ev, 10);
        // 8 × 1.5 × 1.0 = 12.0
        Assert.Equal(12.0, result, 10);
    }

    [Fact]
    // 21 éves autó: kor == 21, ami > 20 → szorzó 2.0
    public void Szamol_KorSzorzo_HuszonegyEvesAutoKettesSzorzo()
    {
        int ev = DateTime.Now.Year - 21; // 21 éves → 2.0
        double result = _service.Szamol("Motor", ev, 10);
        // 8 × 2.0 × 1.0 = 16.0
        Assert.Equal(16.0, result, 10);
    }

    // ─────────────────────────────────────────────────────
    // Súlyossági szorzó tesztek (1–10 skála)
    // ─────────────────────────────────────────────────────

    [Fact]
    // Súlyosság 1 (enyhe hiba): szorzó 0.2
    public void Szamol_Sulyossag1_SzorzóNullaPontKetto()
    {
        int ev = DateTime.Now.Year - 7; // kor szorzó 1.0 (semleges szorzó a teszthez)
        double result = _service.Szamol("Motor", ev, 1);
        // 8 × 1.0 × 0.2 = 1.6
        Assert.Equal(1.6, result, 10);
    }

    [Fact]
    // Súlyosság 2 (enyhe, határérték): szorzó 0.2 – a ≤2 ágba esik
    public void Szamol_Sulyossag2_SzorzóNullaPontKetto()
    {
        int ev = DateTime.Now.Year - 7;
        double result = _service.Szamol("Motor", ev, 2);
        // 8 × 1.0 × 0.2 = 1.6
        Assert.Equal(1.6, result, 10);
    }

    [Fact]
    // Súlyosság 3 (közepes enyhe): szorzó 0.4 – a ≤4 ágba esik
    public void Szamol_Sulyossag3_SzorzóNullaPontNegy()
    {
        int ev = DateTime.Now.Year - 7;
        double result = _service.Szamol("Motor", ev, 3);
        // 8 × 1.0 × 0.4 = 3.2
        Assert.Equal(3.2, result, 10);
    }

    [Fact]
    // Súlyosság 5 (közepes): szorzó 0.6 – a ≤7 ágba esik
    public void Szamol_Sulyossag5_SzorzóNullaPontHat()
    {
        int ev = DateTime.Now.Year - 7;
        double result = _service.Szamol("Motor", ev, 5);
        // 8 × 1.0 × 0.6 = 4.8
        Assert.Equal(4.8, result, 10);
    }

    [Fact]
    // Súlyosság 8 (komoly hiba): szorzó 0.8 – a ≤9 ágba esik
    public void Szamol_Sulyossag8_SzorzóNullaPontNyolc()
    {
        int ev = DateTime.Now.Year - 7;
        double result = _service.Szamol("Motor", ev, 8);
        // 8 × 1.0 × 0.8 = 6.4
        Assert.Equal(6.4, result, 10);
    }

    [Fact]
    // Súlyosság 10 (maximális hiba): szorzó 1.0 – az egyetlen egzakt ág
    public void Szamol_Sulyossag10_SzorzóEgy()
    {
        int ev = DateTime.Now.Year - 7;
        double result = _service.Szamol("Motor", ev, 10);
        // 8 × 1.0 × 1.0 = 8.0
        Assert.Equal(8.0, result, 10);
    }

    // ─────────────────────────────────────────────────────
    // Teljes kombinációs tesztek (reprezentatív esetek)
    // ─────────────────────────────────────────────────────

    [Fact]
    // Karosszéria + öreg autó (25 év) + legsúlyosabb hiba → maximális érték a kategóriában
    public void Szamol_KarosszeriaOregSulyos_MaxEsztimacio()
    {
        int ev = DateTime.Now.Year - 25; // kor szorzó 2.0
        double result = _service.Szamol("Karosszeria", ev, 10); // szorzó 1.0
        // 3 × 2.0 × 1.0 = 6.0
        Assert.Equal(6.0, result, 10);
    }

    [Fact]
    // Motor + nagyon öreg autó + közepes súlyosság – tipikus nagy munka
    public void Szamol_MotorOregKozepesSulyos_HelyyesenSzamol()
    {
        int ev = DateTime.Now.Year - 25; // kor szorzó 2.0
        double result = _service.Szamol("Motor", ev, 5); // szorzó 0.6
        // 8 × 2.0 × 0.6 = 9.6
        Assert.Equal(9.6, result, 10);
    }

    [Fact]
    // Fékberendezés + közepes korú autó (15 év) + erős hiba
    public void Szamol_FekberendezesKozepesAutoSulyosHiba_HellyesenSzamol()
    {
        int ev = DateTime.Now.Year - 15; // kor szorzó 1.5
        double result = _service.Szamol("Fekberendezes", ev, 9); // szorzó 0.8
        // 4 × 1.5 × 0.8 = 4.8
        Assert.Equal(4.8, result, 10);
    }

    [Fact]
    // Futómű + friss autó + enyhébb hiba – minimálishoz közeli eset
    public void Szamol_FutomuUjAutoEnyhHiba_KisEsztimacio()
    {
        int ev = DateTime.Now.Year - 2; // kor szorzó 0.5
        double result = _service.Szamol("Futomu", ev, 2); // szorzó 0.2
        // 6 × 0.5 × 0.2 = 0.6
        Assert.Equal(0.6, result, 10);
    }

    // ─────────────────────────────────────────────────────
    // Hibás bemenet tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // Ismeretlen kategória ArgumentException-t dob
    public void Szamol_IsmeretlenKategoria_ArgumentExceptiont_Dob()
    {
        int ev = DateTime.Now.Year - 5;
        Assert.Throws<ArgumentException>(() => _service.Szamol("Nemletezik", ev, 5));
    }

    [Fact]
    // Üres kategória string ArgumentException-t dob
    public void Szamol_UresKategoriaString_ArgumentExceptiont_Dob()
    {
        int ev = DateTime.Now.Year - 5;
        Assert.Throws<ArgumentException>(() => _service.Szamol("", ev, 5));
    }

    [Fact]
    // Kisbetűs kategória (pl. "motor") ArgumentException-t dob – case-sensitive a switch
    public void Szamol_KisbetusKategoria_ArgumentExceptiont_Dob()
    {
        int ev = DateTime.Now.Year - 5;
        Assert.Throws<ArgumentException>(() => _service.Szamol("motor", ev, 5));
    }

    [Fact]
    // Súlyosság 11 ArgumentException-t dob – tartományon kívül
    public void Szamol_Sulyossag11_ArgumentExceptiont_Dob()
    {
        int ev = DateTime.Now.Year - 5;
        Assert.Throws<ArgumentException>(() => _service.Szamol("Motor", ev, 11));
    }

    [Fact]
    // Súlyosság 100 is ArgumentException-t dob
    public void Szamol_Sulyossag100_ArgumentExceptiont_Dob()
    {
        int ev = DateTime.Now.Year - 5;
        Assert.Throws<ArgumentException>(() => _service.Szamol("Motor", ev, 100));
    }

    // ─────────────────────────────────────────────────────
    // Általános tulajdonság tesztek
    // ─────────────────────────────────────────────────────

    [Fact]
    // Érvényes bemenetre az eredmény mindig pozitív
    public void Szamol_ErvenyesBemenet_MindigPozitivEredmeny()
    {
        int ev = DateTime.Now.Year - 3;
        double result = _service.Szamol("Karosszeria", ev, 1);
        Assert.True(result > 0);
    }

    [Fact]
    // Nagyobb súlyosság nagyobb esztimációt ad (azonos kategória és kor esetén)
    public void Szamol_NagyobbSulyossag_NagyobbEsztimaciot_Ad()
    {
        int ev = DateTime.Now.Year - 7;
        double enyhe = _service.Szamol("Motor", ev, 1);
        double sulyos = _service.Szamol("Motor", ev, 10);
        Assert.True(sulyos > enyhe);
    }

    [Fact]
    // Régebbi autó nagyobb esztimációt ad (azonos kategória és súlyosság esetén)
    public void Szamol_RegibbiAuto_NagyobbEsztimaciot_Ad()
    {
        int ujEv = DateTime.Now.Year - 3;   // kor szorzó 0.5
        int regiEv = DateTime.Now.Year - 25; // kor szorzó 2.0
        double ujAuto = _service.Szamol("Motor", ujEv, 5);
        double regiAuto = _service.Szamol("Motor", regiEv, 5);
        Assert.True(regiAuto > ujAuto);
    }
}
