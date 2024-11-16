## Datenbankcontext und -modell generieren

Paket-Manager-Konsole:

WorklogManagement.Data als Startprojekt einstellen

cd .\WorklogManagement.Data

Scaffold-DbContext -Connection "Server=<Server>;Database=WorklogManagement;User Id=<user>;Password=<password>;TrustServerCertificate=true;" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir ".\Models" -ContextDir ".\Context" -Namespace "WorklogManagement.Data.Models" -ContextNamespace "WorklogManagement.Data.Context" -DataAnnotations -NoOnConfiguring -Force

## migration erzeugen

- Aus WorklogManagement.UI alle dateien mit local.db löschen
- Aus WorklogManagement.Data alles aus ./Migrations löschen
- Paket-Manager-Konsole öffnen
	- WorklogManagement.Data als Startprojekt auswählen
	- cd .\WorklogManagement.Data
	- dotnet ef migrations add InitialCreate --context WorklogManagementContext --startup-project ../WorklogManagement.UI
	- In *_InitialCreate.cs in Up() am Ende unten stehenden Code ergänzen
	- dotnet ef database update --context WorklogManagementContext --startup-project ../WorklogManagement.UI

*_InitialCreate.cs:
```
migrationBuilder.InsertData(
    table: "WorkTimeType",
    columns: new[] { "Id", "Name" },
    values: new object[,]
    {
        { 1, "Office" },
        { 2, "Mobile" },
    });

migrationBuilder.InsertData(
    table: "AbsenceType",
    columns: new[] { "Id", "Name" },
    values: new object[,]
    {
        { 1, "Holiday" },
        { 2, "Vacation" },
        { 3, "Ill" },
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
