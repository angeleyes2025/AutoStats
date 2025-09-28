# 🚗 AutoStats – Digitalna servisna knjižica

**AutoStats** je web aplikacija razvijena u **ASP.NET Core** tehnologiji koja omogućava korisnicima da vode digitalnu evidenciju o vozilima i servisima.  
Cilj aplikacije je da zamijeni papirnu servisnu knjižicu i pruži jednostavan pregled svih troškova i intervencija na vozilu.

---

## 🚀 Funkcionalnosti
- Registracija i prijava korisnika  
- Dodavanje, uređivanje i brisanje vozila  
- Evidencija servisnih intervencija (mali/veliki servis, zamjena guma, klima, itd.)  
- Pregled servisne historije sa troškovima  
- Izvoz servisnih podataka u **PDF** (QuestPDF)  
- Moderno korisničko sučelje sa filtriranjem i bojama po tipu servisa  

---

## 🛠️ Korištene tehnologije
- **ASP.NET Core 8.0**  
- **Entity Framework Core**  
- **C# 12**  
- **QuestPDF** (za generisanje PDF izvještaja)  
- **Bootstrap/Tailwind CSS** (modernizovani frontend dizajn)  
- **SQL Server / SQLite** (baza podataka)  

---

## 📂 Struktura projekta
- `Controllers/` – kontroleri aplikacije  
- `Models/` – modeli podataka (korisnici, vozila, servisi)  
- `Views/` – Razor stranice (frontend)  
- `wwwroot/` – statički resursi (JS, CSS, slike)  
- `Services/` – dodatne klase i servisi (npr. PDF generator)  

---

## ⚙️ Instalacija i pokretanje
1. Kloniraj repozitorij:  
   ```bash
   git clone https://github.com/angeleyes2025/AutoStats.git
   ```
2. Uđi u folder projekta:  
   ```bash
   cd AutoStats
   ```
3. Pokreni aplikaciju:  
   ```bash
   dotnet run
   ```
4. Otvori u pregledniku:  
   ```
   https://localhost:5001
   ```

---

## 📖 Upotreba
1. Registruj novi nalog i prijavi se.  
2. Dodaj vozilo unosom podataka (marka, model, godina, broj šasije, motor, KW...).  
3. Evidentiraj servisne intervencije sa detaljima i troškovima.  
4. Pregledaj historiju servisa i preuzmi izvještaj u PDF formatu.  

---

## 🤝 Autor i licenca
- Autor: **Adel Tahirović**  
- Licenca: MIT  

