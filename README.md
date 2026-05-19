# Sistem Evidență Cursuri și Înscrieri

**Proiect de absolvire – Practică 2026**
**Specializarea:** 61310 Programare și analiza produselor program
**Calificarea:** 351203 Asistent programator

---

## Descriere

Aplicație desktop pentru gestionarea cursanților, cursurilor și înscrierilor
într-un centru de instruire pentru adulți.

**Stack:** C# WinForms (.NET 8) + SQL Server Express (Microsoft.Data.SqlClient)

---

## Structura proiectului

```
SistemEvidentaCursuri/
├── Models/
│   ├── Cursant.cs
│   ├── Curs.cs
│   └── Inscriere.cs
├── Data/
│   └── DatabaseHelper.cs       ← toate metodele CRUD + raport
├── Forms/
│   ├── FormMain.cs             ← fereastra principală cu meniu
│   ├── FormCursanti.cs         ← CRUD cursanți + căutare
│   ├── FormCursantEdit.cs      ← formular adăugare/editare cursant
│   ├── FormCursuri.cs          ← CRUD cursuri + filtrare
│   ├── FormCursEdit.cs         ← formular adăugare/editare curs
│   ├── FormInscrieri.cs        ← gestionare înscrieri
│   └── FormRaport.cs           ← raport sumar + export TXT
├── Helpers/
│   └── Validator.cs            ← validare email, telefon
├── Database/
│   └── create_database.sql     ← script creare BD + date test
└── SistemEvidentaCursuri.sln
```

---

## Setup

### 1. Baza de date

Deschide **SQL Server Management Studio (SSMS)**, conectează-te la `.\SQLEXPRESS`
și execută `Database/create_database.sql`.

### 2. Connection string

Dacă instanța ta SQL Server are alt nume, modifică în `Data/DatabaseHelper.cs`:

```csharp
private static readonly string ConnectionString =
    @"Server=.\SQLEXPRESS;Database=CentruInstruire;Integrated Security=True;TrustServerCertificate=True;";
```

### 3. Rulare

```bash
dotnet run --project SistemEvidentaCursuri
```

sau deschide `.sln` în Visual Studio și apasă F5.

---

## Funcționalități implementate

| # | Cerință | Status |
|---|---------|--------|
| 1 | Proiectarea și popularea BD (E-R, scripturi SQL, date test) | ✅ |
| 2 | Interfața aplicației (meniu, formulare, tabele) | ✅ |
| 3 | Gestionarea Cursanților – CRUD + căutare Nume/Email | ✅ |
| 4 | Gestionarea Cursurilor – CRUD + filtrare Formator/Durată | ✅ |
| 5 | Gestionarea Înscrierilor – înscriere, status plată, anulare, validare duplicate | ✅ |
| 6 | Raport Sumar – Nume cursant \| Nr. înscrieri \| Sumă totală achitată | ✅ |
| 7 | Validare date + tratare erori | ✅ |

### Bonus implementat
- ✅ Sortare descrescătoare după sumă totală (ORDER BY în SQL)
- ✅ Statistici generale (total cursanți, sumă totală, medie/cursant, cursul top)
- ✅ Export raport în format TXT (StreamWriter)
- ✅ Email unic + validare format email + validare format telefon
- ✅ Restricție ștergere cursant/curs cu înscrieri active

---

## Plan săptămânal

| Săpt. | Sarcini | Status |
|-------|---------|--------|
| I | Analiză cerințe, setup VS + SQL Server, structură proiect | ✅ |
| II | Diagrama E-R, scripturi SQL, DatabaseHelper.cs, date test | ✅ |
| III | Clase Model, FormMain cu MenuStrip | ✅ |
| IV | FormCursanti – CRUD complet + căutare + validări | ✅ |
| V | FormCursuri – CRUD complet + filtrare + validare preț | ✅ |
| VI | FormInscrieri – adăugare, afișare, anulare, validare duplicate | ✅ |
| VII | FormRaport + statistici + export TXT + testare completă | ✅ |
| VIII | Raport de practică + PowerPoint + susținere | 🔄 |
