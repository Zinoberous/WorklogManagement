using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Helper;
using WorklogManagement.DataAccess.Context;

Console.Title = "WorklogManagement.API";

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

#if STAGING
config.AddJsonFile("local.settings.dev.json", true);
#elif PROD
config.AddJsonFile("local.settings.prod.json", true);
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

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "WorklogManagement", Version = "v1" });
    }
);

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
app.UseSwaggerUI();

app.Run();
