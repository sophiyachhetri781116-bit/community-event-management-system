# Community Event Management System

A role-based web application for managing community events, venues, activities and participant registrations. Admins manage the data; registered users browse upcoming events, filter them and register online.

Built with **ASP.NET Core (.NET 10)**, **Blazor Server**, **Entity Framework Core** and **SQL Server (LocalDB)**.

## Features
- Role-based access (Admin / User) using ASP.NET Core Identity, with email confirmation, 2FA and passkey support from the Identity scaffold
- Event listing with filters by date, venue, activity and activity type
- Admin CRUD for events, venues, activities and participants
- Many-to-many relationships: events ↔ venues and events ↔ activities
- Registration rules enforced in the service layer: no duplicate registrations, and sign-ups stop when the combined venue capacity is reached
- Generic service layer (`IService<T>`, `BaseService<T>`) with dependency injection and custom exceptions
- EF Core migrations and 14 MSTest unit tests (in-memory database)

## Screenshots
<!-- Add your screenshots here, e.g. ![Events page](screenshots/events.png) -->

## Getting started

**Requirements:** .NET 10 SDK, SQL Server LocalDB (included with Visual Studio), and the EF Core tools (`dotnet tool install --global dotnet-ef`).

```
git clone https://github.com/sophiyachhetri781116-bit/community-event-management-system.git
cd community-event-management-system/CommunityEventManagement
dotnet ef database update
dotnet run
```

Then open `https://localhost:7225`. The database must be migrated before the first run, because the app seeds its accounts on startup.

In Visual Studio: open `CommunityEventManagement.slnx`, run `Update-Database` in the Package Manager Console, then press F5.

Run the tests with `dotnet test` from the solution folder.

## Demo accounts
| Role | Email | Password |
|---|---|---|
| Admin | admin@cems.com | Admin@123 |
| User | user@cems.com | User@123 |

These are demo credentials for local use only.

## Project structure
- `Data/` – models, DbContext, migrations
- `Services/` – generic base service and event, registration, participant, venue and activity services
- `Exceptions/` – custom business-rule exceptions
- `Components/Pages/` – Blazor pages for the application
- `Components/Account/` – Identity pages (ASP.NET Core scaffold)
- `CommunityEventManagement.Tests/` – MSTest unit tests

## Known limitations
- The venue, activity and participant list pages require login but are not restricted to Admin (only the navigation hides them)
- Cancelling a registration deletes it instead of setting its status to Cancelled
- Duplicate and capacity checks are in code only, with no database-level unique constraint
- The email sender is a no-op, so the confirmation link is shown on screen after registering
- Database calls are synchronous

## Author
Sophiya Chhetri

## License
MIT
