# Autószerelő Műhely — Projektleírás

Blazor WebAssembly frontend + ASP.NET Core Web API backend, MongoDB adatbázissal.

---

## Szükséges csomagok (NuGet)

### AutoszereloMuhely (Backend API)
| Csomag | Verzió |
|---|---|
| `MongoDB.Driver` | 3.4.0 |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.3 |
| `Microsoft.AspNetCore.OpenApi` | 10.0.3 |
| `Swashbuckle.AspNetCore` | 10.1.7 |

### AutoszereloMuhely.Client (Blazor WASM)
Nincs extra NuGet csomag — a standard Blazor WebAssembly keretrendszer elegendő.

### AutoszereloMuhely.Tests (Tesztek)
| Csomag | Verzió |
|---|---|
| `xunit` | 2.9.3 |
| `xunit.runner.visualstudio` | 3.1.4 |
| `NSubstitute` | 5.3.0 |
| `coverlet.collector` | 6.0.4 |
| `Microsoft.NET.Test.Sdk` | 17.14.1 |

---

## MongoDB beállítása (új gépen)

### 1. MongoDB Community Server telepítése
1. Menj a [https://www.mongodb.com/try/download/community](https://www.mongodb.com/try/download/community) oldalra
2. Töltsd le a **MongoDB Community Server** legfrissebb verzióját (Windows MSI)
3. Futtasd a telepítőt — hagyd meg az alapértelmezett beállításokat
4. Győződj meg róla, hogy az **"Install MongoDB as a Service"** opció be van pipálva
5. A telepítés után a MongoDB automatikusan elindul a `localhost:27017` porton

### 2. MongoDB Compass telepítése (opcionális, GUI)
1. Töltsd le a [https://www.mongodb.com/try/download/compass](https://www.mongodb.com/try/download/compass) oldalról
2. Telepítsd és nyisd meg
3. A kapcsolódási URI: `mongodb://localhost:27017` — kattints **Connect**-re
4. Kattints a **"Create database"** gombra, és hozd létre az `autoszerelo` adatbázist
   - Első gyűjtemény neve: `ugyfelek`
   - A `munkak` gyűjtemény automatikusan létrejön az első mentéskor

> **Megjegyzés:** Az adatbázist és a gyűjteményeket az alkalmazás automatikusan létrehozza az első íráskor, így a Compass lépés opcionális.

### 3. Kapcsolódási beállítás
A backend connection string az `AutoszereloMuhely/appsettings.json` fájlban található:
```json
"MongoDB": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "autoszerelo"
}
```
Ha a MongoDB más porton fut, vagy jelszó szükséges, csak ezt a fájlt kell módosítani.

---

## A program indítása és használata

### Előfeltételek
- [.NET 10 SDK](https://dotnet.microsoft.com/download) telepítve
- MongoDB fut a gépen (lásd fent)

### Indítás

A megoldás két projektet tartalmaz, amelyeket **egyszerre** kell futtatni:

#### 1. Backend API indítása
```bash
cd AutoszereloMuhely
dotnet run
```
Az API alapértelmezetten a `https://localhost:7xxx` és `http://localhost:5xxx` portokon indul el (a pontos port a `launchSettings.json`-ban látható). A Swagger UI fejlesztési módban elérhető a `/swagger` útvonalon.

#### 2. Frontend (Blazor WASM) indítása
```bash
cd AutoszereloMuhely.Client
dotnet run
```
A Blazor frontend a böngészőben nyílik meg (pl. `https://localhost:7xxx`).

> **Tipp:** Visual Studio-ban a megoldás megnyitása után az `AutoszereloMuhely.slnx` fájlból mindkét projekt egyszerre indítható a **Multiple Startup Projects** konfigurációval.

### Az alkalmazás funkciói

#### Bejelentkezés / Regisztráció
- `/bejelentkezes` — Felhasználói bejelentkezés (JWT alapú)
- `/regisztracio` — Új felhasználói fiók létrehozása
- `/iroda/login` — Irodai dolgozói bejelentkezés
- `/profil` — Bejelentkezett felhasználó profiljának megtekintése és szerkesztése

#### Irodai felület (`/iroda`) — Dolgozó / Admin szerepkör
- `/iroda/ugyfelek` — Megrendelők listázása, keresése, hozzáadása, törlése
- `/iroda/ugyfelek/szerkesztes/{id}` — Ügyfél adatainak szerkesztése
- `/iroda/munkak` — Munkák listázása, keresése, hozzáadása, állapotléptetés, törlés
- `/iroda/munkak/{id}` — Egy munka részletes adatai és szerkesztése

#### Admin felület (`/admin`) — Admin szerepkör
- `/admin/felhasznalok` — Felhasználók kezelése, szerepkörök módosítása

#### Megrendelői felület (`/megrendelo`)
- `/megrendelo/bejelentkezes` — Megrendelői bejelentkezés
- `/megrendelo/munkak/{ugyfelId}` — Az ügyfélhez tartozó munkák megtekintése (csak olvasható)

#### Szerepkörök
| Szerepkör | Hozzáférés |
|---|---|
| `Admin` | Teljes hozzáférés, felhasználókezelés |
| `Dolgozo` | Irodai felület (ügyfelek, munkák) |
| `Ugyfel` | Saját munkák megtekintése |

### Tesztek futtatása
```bash
cd AutoszereloMuhely.Tests
dotnet test
```
