## Datenbankcontext und -modell generieren

Scaffold-DbContext -Connection "Server=<Server>;Database=WorklogManagement;User Id=<user>;Password=<password>;TrustServerCertificate=true;" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir ".\Models" -ContextDir ".\Context" -Namespace "WorklogManagement.DataAccess.Models" -ContextNamespace "WorklogManagement.DataAccess.Context" -DataAnnotations -NoOnConfiguring -Force

## migration erzeugen

- cd .\WorklogManagement.DataAccess
- dotnet ef migrations add InitialCreate --context WorklogManagementContext --startup-project ../WorklogManagement.API

- *_InitialCreate.cs:
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
```

## local.db erzeugen

- cd .\WorklogManagement.DataAccess
- dotnet ef database update --context WorklogManagementContext --startup-project ../WorklogManagement.API
