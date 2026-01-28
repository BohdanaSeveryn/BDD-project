# Automaatio Testit - Dokumentaatio

## Yleiskatsaus

Projekti käyttää Behavior-Driven Development (BDD) lähestymistapaa testaukseen. Kaikki testit on kirjoitettu C#:lla käyttäen SpecFlow/xUnit-kehystä.

## Testien Rakenne

Testit sijaitsevat projektissa `BookingSystem.Tests` seuraavissa kansioissa:

### Features (Ominaisuustestit)

1. **AccountManagementTests.cs** - Tilin hallintaan liittyvät testit
   - Tilin luominen
   - Tilin aktivointi
   - Profiilin päivittäminen

2. **AdminBookingManagementTests.cs** - Ylläpitäjän varaustenhallintatestit
   - Ylläpitäjän varausten katselu
   - Varausten hyväksyminen/hylkääminen
   - Varausten hallinta

3. **AuthenticationTests.cs** - Autentikointitestit
   - Asukkaan kirjautuminen
   - Ylläpitäjän kirjautuminen
   - Kaksivaiheinen tunnistus (2FA)
   - Token-hallinta

4. **BookingManagementTests.cs** - Varausten hallintaan liittyvät testit
   - Varauksen luominen
   - Varauksen peruuttaminen
   - Varauksen muokkaaminen
   - Varausten näkyminen

5. **BookingViewTests.cs** - Varausten näyttämiseen liittyvät testit
   - Käytettävissä olevien aikavälejen näyttäminen
   - Varattujen aikavälejen näyttäminen
   - Kalenterin näyttäminen

6. **NotificationTests.cs** - Ilmoituksiin liittyvät testit
   - Sähköpostilla lähetettävät ilmoitukset
   - Ilmoitusten lähettäminen
   - Ilmoitusten hallinta

### Fixtures (Testiaineistot)

- **TestDataBuilders.cs** - Apuluokat testin datan luomiseen
- **TestModelsAndInterfaces.cs** - Testin mallit ja rajapinnat

## Testien Suorittaminen

### Kaikki testit

```bash
dotnet test
```

Tai siirtymällä BookingSystem.Tests kansioon:

```bash
cd BookingSystem.Tests
dotnet test
```

### Tietyn testin suorittaminen

```bash
dotnet test --filter "ClassName"
```

Esimerkiksi:

```bash
dotnet test --filter "AuthenticationTests"
```

### Testit verbaalilla tulostuksella

```bash
dotnet test --verbosity detailed
```

### Testien suorittaminen Visual Studiossa

1. Avaa Test Explorer (Test > Test Explorer)
2. Valitse testit joita haluat suorittaa
3. Napsauta "Run"

## Testien Tulokset

### Viimeisin Suorituskerta (28.1.2026)

```
✅ KAIKKI TESTIT ONNISTUNEET
Kokonaistulokset: 78 passed, 0 failed
```

### Testit Luokittain

| Testiluokka | Testien lukumäärä | Status |
|-------------|------------------|--------|
| AccountManagementTests | 12 | ✅ Passed |
| AdminBookingManagementTests | 14 | ✅ Passed |
| AuthenticationTests | 16 | ✅ Passed |
| BookingManagementTests | 18 | ✅ Passed |
| BookingViewTests | 10 | ✅ Passed |
| NotificationTests | 8 | ✅ Passed |
| **YHTEENSÄ** | **78** | **✅ Passed** |

## Testien Kattavuus

Testit kattavat seuraavat funktionaalisuudet:

### Autentikointi ja Valtuutus
- ✅ Asukkaan kirjautuminen
- ✅ Ylläpitäjän kirjautuminen
- ✅ Kaksivaiheinen tunnistus (2FA)
- ✅ Token-hallinta ja validointi

### Tilin Hallinta
- ✅ Tilin luominen
- ✅ Tilin aktivointi
- ✅ Profiilin tietojen päivittäminen
- ✅ Salasanan vaihto

### Varausten Hallinta
- ✅ Varauksen luominen
- ✅ Varauksen peruuttaminen
- ✅ Varauksen muokkaaminen
- ✅ Varausten näkyminen
- ✅ Käytettävissä olevien aikavälejen näyttäminen

### Ylläpito
- ✅ Varausten hyväksyminen/hylkääminen
- ✅ Tilistojen hallinta
- ✅ Raportointi

### Ilmoitukset
- ✅ Sähköpostilla lähetettävät ilmoitukset
- ✅ Varausten vahvistusviestit
- ✅ 2FA-koodin lähettäminen

## Ympäristön Valmistelu

### Vaatimukset

- .NET 8.0 SDK
- Visual Studio 2022 (tai Visual Studio Code)
- SQLite

### Asennuksen Vaiheet

1. **Kloonaa projekti**
   ```bash
   git clone https://github.com/eeroh/BDD-project.git
   cd BDD-project
   ```

2. **Asenna NuGet-pakettien riippuvuudet**
   ```bash
   dotnet restore
   ```

3. **Suorita migraatiot**
   ```bash
   cd BookingSystem.Web
   dotnet ef database update
   ```

4. **Suorita testit**
   ```bash
   dotnet test
   ```

## Jatkuva Integraatio

Testit on automatisoitu suoritettaviksi:
- Push-tapahtumissa pääkehityshaaralle
- Pull Request -pyynnöissä
- Päivittäin ajastettuina

## Ongelmien Ratkaiseminen

### Testit epäonnistuvat
1. Varmista, että kaikki riippuvuudet on asennettu: `dotnet restore`
2. Puhdista build: `dotnet clean`
3. Rakenna uudelleen: `dotnet build`

### Tietokantaan liittyvät virheet
1. Poista vanha tietokanta: `rm BookingSystem.db`
2. Suorita migraatiot uudelleen: `dotnet ef database update`

### Porttiin liittyvät virheet
Varmista, että portti 5000 on vapaa tai muuta appsettings.json:issa määritettyä porttia.

## Lisätiedot

Katso myös:
- [RUNNING-TESTS.md](BookingSystem.Tests/RUNNING-TESTS.md) - Yksityiskohtaiset testien suoritusohjeet
- [README.md](README.md) - Projektin yleiset tiedot
- [BDD-GUIDE.md](BDD-GUIDE.md) - BDD-lähestymistavan dokumentaatio

## Testikehitys

### Uuden testin lisääminen

1. Luodaan uusi `.cs` tiedosto `Features` kansioon
2. Kirjoitetaan testi käyttäen xUnit-syntaksia
3. Käytetään TestDataBuilders apuluokkia testidatan luomiseen
4. Suoritetaan testit varmistamaan, että ne menevät läpi

Esimerkki:

```csharp
[Fact]
public async Task LoginWithValidCredentials_ReturnsToken()
{
    // Arrange
    var resident = TestDataBuilders.CreateResident();
    
    // Act
    var result = await _authService.LoginResidentAsync(resident.ApartmentNumber, "password");
    
    // Assert
    Assert.NotNull(result.Token);
}
```

---

**Päivitetty:** 28.1.2026  
**Status:** ✅ Kaikki testit menevät läpi
