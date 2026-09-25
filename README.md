# Sportcsapat nyilvántartó

Ifjúsági sportcsapat meccs- és statisztikai nyilvántartó. A backend egy ASP.NET Core
(.NET 9) minimal API, ami a csapatokat, játékosokat, edzőket, meccseket és az edzéseket
tartja nyilván.

## Indítás

```bash
cd backend/SportcsapatApi
dotnet run --launch-profile http
```

Az API a `http://localhost:5152` címen indul. Fejlesztői módban az OpenAPI leírás a
`/openapi/v1.json` címen érhető el. A végpontok kipróbálásához a
[SportcsapatApi.http](backend/SportcsapatApi/SportcsapatApi.http) fájlban kész kérések vannak.

## Végpontok

| Metódus | Útvonal | Leírás |
| --- | --- | --- |
| GET | `/api/teams` | Csapatok listája |
| GET | `/api/teams/{id}` | Egy csapat az edzőjével és a játékosaival |
| POST | `/api/teams` | Új csapat |
| POST | `/api/teams/{id}/coach` | Edző beállítása a csapathoz |
| GET | `/api/players?teamId=` | Játékosok (csapatra szűrhető) |
| GET | `/api/players/{id}` | Egy játékos adatai |
| POST | `/api/players` | Új játékos |
| PUT | `/api/players/{id}` | Játékos adatainak módosítása (szülői felület) |
| DELETE | `/api/players/{id}` | Játékos törlése a statisztikáival együtt |
| GET | `/api/players/{id}/summary` | Szezonösszesítő: gólok, percek, edzéslátogatottság |
| GET | `/api/matches?teamId=` | Meccsek és eredmények (nyilvános) |
| GET | `/api/matches/{id}` | Egy meccs a játékosstatisztikákkal |
| POST | `/api/matches` | Új meccs |
| PUT | `/api/matches/{id}` | Meccs eredményének javítása |
| GET | `/api/matches/{id}/stats` | A meccs játékosstatisztikái |
| POST | `/api/matches/{id}/stats` | Játékos statisztikájának rögzítése/felülírása |
| GET | `/api/trainings?teamId=` | Edzések |
| POST | `/api/trainings` | Új edzés |
| GET | `/api/trainings/{id}/attendance` | Jelenléti ív |
| POST | `/api/trainings/{id}/attendance` | Jelenlét rögzítése (egyszerre több játékosra) |

## Felépítés

```
backend/SportcsapatApi/
  Program.cs            – szolgáltatások, CORS, végpontok bekötése
  Models/Entities.cs    – Team, Coach, Player, Match, PlayerMatchStat, Training, TrainingAttendance
  Models/Dtos.cs        – kérés/válasz típusok
  Data/SportStore.cs    – ideiglenes memóriabeli adattár, mintaadatokkal
  Endpoints/            – a végpontok csoportonként
```

## Hátralévő lépések

- Adatbázis bekötése (`Csapatadatbázis.sql`) az in-memory `SportStore` helyett
- Bejelentkezés és jogosultságok: edző (rögzítés) és szülő (saját gyerek adatai)
- Frontend weboldal a nyilvános eredményekhez és a két felülethez
