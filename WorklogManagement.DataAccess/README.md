## Datenbankcontext und -models generieren

Scaffold-DbContext -Connection "Server=lexnarf.dns.navy,52341;Database=StageWorklogManagement;User Id=Lex;Password=<password>;TrustServerCertificate=true;" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir ".\Models" -ContextDir ".\Context" -Namespace "WorklogManagement.DataAccess.Models" -ContextNamespace "WorklogManagement.DataAccess.Context" -DataAnnotations -NoOnConfiguring -Force
