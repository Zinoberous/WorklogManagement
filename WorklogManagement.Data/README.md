## Datenbankcontext und -modell generieren

Paket-Manager-Konsole:

WorklogManagement.Data asl Startprojekt einstellen

cd .\WorklogManagement.Data

Scaffold-DbContext -Connection "Server=<Server>;Database=WorklogManagement;User Id=<user>;Password=<password>;TrustServerCertificate=true;" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir ".\Models" -ContextDir ".\Context" -Namespace "WorklogManagement.Data.Models" -ContextNamespace "WorklogManagement.Data.Context" -DataAnnotations -NoOnConfiguring -Force

## migration erzeugen

- Aus WorklogManagement.UI alle dateien mit local.db l�schen
- Aus WorklogManagement.Data alles aus ./Migrations l�schen
- Paket-Manager-Konsole �ffnen
	- WorklogManagement.Data als Startprojekt ausw�hlen
	- cd .\WorklogManagement.Data
	- dotnet ef migrations add InitialCreate --context WorklogManagementContext --startup-project ../WorklogManagement.UI
	- In *_InitialCreate.cs in Up() am Ende unten stehenden Code erg�nzen
	- dotnet ef database update --context WorklogManagementContext --startup-project ../WorklogManagement.UI

*_InitialCreate.cs:
```
migrationBuilder.InsertData(
    table: "CalendarEntryType",
    columns: new[] { "Id", "Name" },
    values: new object[,]
    {
        { 1, "WorkTime" },
        { 2, "Office" },
        { 3, "Mobile" },
        { 4, "Holiday" },
        { 5, "Vacation" },
        { 6, "TimeCompensation" },
        { 7, "Ill" },
    });

migrationBuilder.InsertData(
    table: "TicketStatus",
    columns: new[] { "Id", "Name" },
    values: new object[,]
    {
        { 1, "Todo" },
        { 2, "Running" },
        { 3, "Paused" },
        { 4, "Blocked" },
        { 5, "Done" },
        { 6, "Canceled" },
        { 7, "Continuous" },
    });