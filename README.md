# TooliRent API

TooliRent är ett REST API byggt i **ASP.NET Core 8**, designat för att hantera uthyrning av verktyg i en makerspace-miljö.  
API:et gör det möjligt för medlemmar att registrera sig, boka verktyg, hämta och lämna tillbaka verktyg, medan administratörer kan hantera användare, verktyg, kategorier och statistik.  

Projektet använder **N-tier arkitektur** med tydlig separation mellan lager samt moderna designmönster (Repository & Service).

---

## Arkitektur

```text
Solution
│
├── Api                → Presentation layer (Controllers, Swagger, Auth setup)
├── Application        → Application layer (Services, DTOs, Validators, Mapping profiles)
├── Domain             → Domain layer (Entities, Interfaces)
└── Infrastructure     → Infrastructure layer (DbContext, EF Core Migrations, Repositories, Seed data)
```
* Api
Ansvarar för att exponera REST-endpoints, autentisering (JWT), auktorisering, Swagger-dokumentation.

* Application
Innehåller tjänster (services) och DTOs. Här finns logik för bokningar, verktygshantering och användarflöden.

* Domain
Håller entiteterna (t.ex. AppUser, Tool, Category, Booking) och kontrakt (interfaces).

* Infrastructure
Implementerar dataåtkomst med Entity Framework Core, repository pattern, migrations och seed-data.

---

## Kom igång lokalt
### Förutsättningar
* .NET 8 SDK
* SQL Server eller LocalDB (för utveckling)
* Visual Studio 2022 / Rider / VS Code

### Körning
1. Klona projektet:

```bash
git clone https://github.com/<ditt-repo>/TooliRent.git
cd TooliRent
```
2. Uppdatera Api/appsettings.json med rätt connection string:
```json
"ConnectionStrings": {
  "Default": "Server=(localdb)\\MSSQLLocalDB;Database=TooliRentDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```
3. Kör migrationer för att skapa databasen:

```bash
dotnet ef database update --project Infrastructure --startup-project Api --context TooliRentDbContext
```
4. Starta API:t:

```bash
dotnet run --project Api
```
5. Öppna Swagger-dokumentationen i webbläsaren:
https://localhost:5001/swagger

### Seed-data
Vid första körningen seedas:
* Roller: Admin, Member
* Användare:
  * admin@tooli.local / Admin123!
  * member@tooli.local / Member123!
* Kategorier: Handverktyg, Elverktyg
* Verktyg: Hammare, Skruvdragare, Sticksåg

---

## API-endpoints
### Autentisering
* POST /api/auth/register – registrera ny användare
* POST /api/auth/login – logga in, få JWT token
* POST /api/auth/refresh – förnya access-token

### Verktyg
* GET /api/tools – lista alla verktyg (kan filtreras på kategori, status, tillgänglighet)
* GET /api/tools/{id} – hämta specifikt verktyg
* POST /api/tools (Admin) – skapa nytt verktyg
* PUT /api/tools/{id} (Admin) – uppdatera verktyg
* DELETE /api/tools/{id} (Admin) – ta bort verktyg

### Kategorier
* GET /api/categories – lista kategorier
* POST /api/categories (Admin) – skapa kategori
* PUT /api/categories/{id} (Admin) – uppdatera kategori
* DELETE /api/categories/{id} (Admin) – ta bort kategori

### Bokningar
* GET /api/bookings – lista inloggad medlems bokningar
* POST /api/bookings – skapa en ny bokning (ett eller flera verktyg, tidsperiod)
* DELETE /api/bookings/{id} – avboka bokning
* POST /api/bookings/{id}/pickup – markera som uthämtad
* POST /api/bookings/{id}/return – markera som återlämnad

### Admin
* PUT /api/admin/users/{id}/activate – aktivera användare
* PUT /api/admin/users/{id}/deactivate – inaktivera användare
* GET /api/admin/statistics – statistik över användning och uthyrning

---

## Datamodeller (DTOs)
### Auth
```csharp
public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string AccessToken, DateTime ExpiresAt, string RefreshToken, Guid UserId);
public record RefreshRequest(string RefreshToken, Guid UserId);
```
### Tools & Categories
```csharp
public record ToolReadDto(Guid Id, string Name, string CategoryName, ToolStatus Status, string? SerialNumber, string? Condition, string? Description);
public record ToolCreateUpdateDto(string Name, Guid CategoryId, ToolStatus Status, string? SerialNumber, string? Condition, string? Description);

public record CategoryReadDto(Guid Id, string Name);
public record CategoryCreateDto(string Name);
public record CategoryUpdateDto(string Name);
```
### Bookings
```csharp
public record BookingItemReadDto(Guid ToolId, string ToolName);
public record BookingReadDto(Guid Id, DateTime StartAt, DateTime EndAt, BookingStatus Status, DateTime? PickedUpAt, DateTime? ReturnedAt, IReadOnlyList<BookingItemReadDto> Items);
public record BookingCreateDto(DateTime StartAt, DateTime EndAt, IReadOnlyList<Guid> ToolIds);
```

---

## Teknisk stack
* ASP.NET Core 8 Web API
* Entity Framework Core 8 (SQL Server, Code First Migrations)
* ASP.NET Core Identity (med Guid som nyckel)
* JWT Bearer Authentication + rollbaserad auktorisering
* AutoMapper för entity/DTO-mappning
* FluentValidation för DTO-validering
* Swagger / OpenAPI för dokumentation
