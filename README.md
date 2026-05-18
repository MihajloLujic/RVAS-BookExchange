# RVAS BookExchange

Platforma za pozajmljivanje i razmenu knjiga.

Projekat je rađen za predmet **Razvoj veb aplikacija i servisa**.

## Tehnologije

- ASP.NET Core MVC
- C#
- MongoDB
- Razor Views
- Cookie Authentication
- Git / GitHub

## Funkcionalnosti

### Korisnici

- Registracija korisnika
- Prijava korisnika
- Odjava korisnika
- Izbor omiljenih žanrova prilikom registracije
- Čuvanje korisnika u MongoDB bazi
- Lozinke se čuvaju kao hash, ne kao običan tekst

### Knjige / oglasi

- Prikaz dostupnih knjiga
- Dodavanje nove knjige
- Upload fotografije knjige
- Prikaz detalja knjige
- Pregled mojih oglasa
- Izmena oglasa
- Brisanje oglasa
- Datum objave
- Datum poslednje izmene

### Pretraga i filteri

- Pretraga po naslovu knjige
- Pretraga po autoru
- Filter po žanru
- Filter po gradu/lokaciji
- Paginacija: 12 knjiga po stranici

### Zahtevi

- Slanje zahteva za pozajmljivanje ili razmenu
- Pregled pristiglih zahteva
- Pregled poslatih zahteva
- Prihvatanje zahteva
- Odbijanje zahteva
- Opcioni komentar uz odgovor

### Poruke

- Pokretanje razgovora između vlasnika oglasa i zainteresovanog korisnika
- Slanje poruka
- Pregled svih razgovora
- Pregled pojedinačnog razgovora
- Poruke se čuvaju u MongoDB bazi
- Nije korišćen real-time sistem

## Pokretanje projekta

### 1. Pokrenuti MongoDB

MongoDB se pokrece rucno komandom:

```bash
"C:\Program Files\MongoDB\Server\8.0\bin\mongod.exe"

### URL

- ovo su url-ovi
/Account/Register
/Account/Login
/Books
/Books/Create
/MyListings
/Requests/Incoming
/Requests/Sent
/Messages