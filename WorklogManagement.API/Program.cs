using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WorklogManagement.API.Helper;
using WorklogManagement.DataAccess.Context;

#if STAGING
Console.Title = "StageWorklogManagement.API";
#elif PRODUCTION
Console.Title = "WorklogManagement.API";
#else
Console.Title = "WorklogManagement.API";
#endif

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

#if STAGING
config.AddJsonFile("local.settings.staging.json", true);
#elif PRODUCTION
config.AddJsonFile("local.settings.production.json", true);
#endif

config.AddJsonFile("local.settings.json", true);

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
services.Configure<SwaggerGenOptions>(options =>
{
    options.SwaggerGeneratorOptions.Servers =
    [
        new() { Url = "/stage-worklog-management/api" }
    ];
});
#elif PRODUCTION
services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "WorklogManagement", Version = "v1" });
    }
);
services.Configure<SwaggerGenOptions>(options =>
{
    options.SwaggerGeneratorOptions.Servers =
    [
        new() { Url = "/worklog-management/api" }
    ];
});
#else
services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "WorklogManagement", Version = "v1" });
    }
);
services.Configure<SwaggerGenOptions>(options =>
{
    options.SwaggerGeneratorOptions.Servers =
    [
        new() { Url = "/worklog-management/api" }
    ];
});
#endif

services.AddDbContext<WorklogManagementContext>
(
    options =>
    {
        options.UseSqlServer(config.GetConnectionString("WorklogManagement")!);
    }
);

var app = builder.Build();

app.MapControllers();

app.UseCors();

app.UseSwagger();

app.UseSwaggerUI();

app.Run();
