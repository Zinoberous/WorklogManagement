Datenbankcontext und -modell generieren:
Scaffold-DbContext -Connection "Server=<Server>;Database=WorklogManagement;User Id=<user>;Password=<password>;TrustServerCertificate=true;" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir ".\Models" -ContextDir ".\Context" -Namespace "WorklogManagement.DataAccess.Models" -ContextNamespace "WorklogManagement.DataAccess.Context" -DataAnnotations -NoOnConfiguring -Force

local.db erzeugen:
dotnet ef migrations add InitialCreate --context WorklogManagementContext --startup-project ../WorklogManagement.API
dotnet ef database update --context WorklogManagementContext --startup-project ../WorklogManagement.API
