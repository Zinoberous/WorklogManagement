using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Helper;
using WorklogManagement.DataAccess.Context;

#if STAGING
Console.Title = "StageWorklogManagement.API";
#elif PRODUCTION
Console.Title = "ProdWorklogManagement.API";
#else
Console.Title = "WorklogManagement.API";
#endif

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

#if STAGING
config.AddJsonFile("local.settings.staging.json", true);
#elif PRODUCTION
config.AddJsonFile("local.settings.production.json", true);
#else
config.AddJsonFile("local.settings.json", true);
#endif

ConfigHelper.Initialize(config);

var services = builder.Services;

services.AddControllers();

services.AddCors
(
    options =>
    {
        options.AddDefaultPolicy
        (
            builder =>
            {
                builder.SetIsOriginAllowed(_ => true);
                builder.AllowCredentials();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            }
        );
    }
);

#if STAGING

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "StageWorklogManagement", Version = "v1" });
    }
);

#elif PRODUCTION

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "ProdWorklogManagement", Version = "v1" });
    }
);

#else

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "WorklogManagement", Version = "v1" });
    }
);

#endif

services.AddDbContext<WorklogManagementContext>
(
    options =>
    {
#if DEBUG
        //options.UseSqlite("Data Source=local.db");
        options.UseSqlServer(config.GetConnectionString("WorklogManagement")!);
#else
        options.UseSqlServer(config.GetConnectionString("WorklogManagement")!);
#endif
    }
);

var app = builder.Build();

app.MapControllers();

app.UseCors();

app.UseSwagger();

#if STAGING
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("https://lexnarf.dns.navy/stage-worklog-management/api/swagger/v1/swagger.json", "StageWorklogManagement API v1");
    c.RoutePrefix = string.Empty;
});
#elif PRODUCTION
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("https://lexnarf.dns.navy/worklog-management/api/swagger/v1/swagger.json", "ProdWorklogManagement API v1");
    c.RoutePrefix = string.Empty;
});
#else
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("https://lexnarf.dns.navy/worklog-management/api/swagger/v1/swagger.json", "WorklogManagement API v1");
    c.RoutePrefix = string.Empty;
});

#endif

app.Run();
