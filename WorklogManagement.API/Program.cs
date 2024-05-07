using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API;
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

services.AddMvc();

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "StageWorklogManagement", Version = "v1" });
        options.DocumentFilter<SwaggerBasePathFilter>("/stage-worklog-management/api");

    }
);

#elif PRODUCTION

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "ProdWorklogManagement", Version = "v1" });
        options.DocumentFilter<SwaggerBasePathFilter>("/worklog-management/api");
    }
);

#else

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "WorklogManagement", Version = "v1" });
        options.DocumentFilter<SwaggerBasePathFilter>("/worklog-management/api");

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

app.UseSwaggerUI(c =>
{
#if STAGING
    c.SwaggerEndpoint("/stage-worklog-management/api/swagger/v1/swagger.json", "StageWorklogManagement API v1");
#elif PRODUCTION
    c.SwaggerEndpoint("/worklog-management/api/swagger/v1/swagger.json", "ProdWorklogManagement API v1");
#else
    c.SwaggerEndpoint("/worklog-management/api/swagger/v1/swagger.json", "WorklogManagement API v1");
#endif
    c.RoutePrefix = "swagger";
});

app.Run();

namespace WorklogManagement.API
{
    public class SwaggerBasePathFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var config = ConfigHelper.Config;

            string scheme = config.GetValue<string>("PubScheme");

            string hostPath = config.GetValue<string>("PubHost");

            string basePath = config.GetValue<string>("PubBase");

            var paths = new OpenApiPaths();

            foreach (var (key, value) in swaggerDoc.Paths)
            {
                paths.Add(key.Replace($"{scheme}://{hostPath}", $"{scheme}://{hostPath}{basePath}"), value);
            }

            swaggerDoc.Paths = paths;
        }
    }
}
