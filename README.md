Sportcsapat Edzés- és Jelenlét-nyilvántartó Rendszer

A projekt lényege

Ez a webalkalmazás egy ifjúsági sportcsapat edzés- és mérkőzés-adatainak digitális nyilvántartására készült. A rendszer segítségével az edzők gyorsan rögzíthetik az edzéseken való részvételt és a mérkőzés-statisztikákat, míg a szülők bármikor nyomon követhetik gyermekük részvételét és teljesítményét egy egyszerű, reszponzív felületen.

A projekt egy RESTful backend API-ból (ASP.NET Core) és egy webes frontendből áll, amelyek egy közös MySQL adatbázison keresztül kommunikálnak. A weboldal asztali gépen és mobilon egyaránt jól használható.

Funkciók

 Bejelentkezés / szerepkörök
- Regisztráció és bejelentkezés saját fiókkal
- Két szerepkör: **edző** (admin jogosultság) és **szülő** (korlátozott, csak saját gyermek adataira vonatkozó betekintés)
- Jogosultság-alapú hozzáférés: a szülő csak a saját gyermeke adatait látja, az edző mindent kezel

 Edzői (admin) funkciók
- Csapatok létrehozása és kezelése (korosztály, edző neve)
- Játékosok felvétele, adatainak szerkesztése, csapathoz rendelése
- Edzések létrehozása (dátum, helyszín)
- Jelenlét gyors rögzítése edzésenként (pipálható lista)
- Mérkőzések rögzítése (dátum, ellenfél, eredmény)
- Játékos-statisztikák rögzítése mérkőzésenként (gólok, sárga/piros lap)
- Áttekintő statisztikák (pl. leggyakoribb hiányzók, legeredményesebb játékosok)
Szülői funkciók
- Bejelentkezés saját fiókkal
- Gyermek jelenlét-történetének megtekintése edzésenként
- Csapat mérkőzés-eredményeinek megtekintése
- Gyermek egyéni statisztikáinak megtekintése (gólok, lapok)
- Következő edzés/mérkőzés időpontjának megtekintése

