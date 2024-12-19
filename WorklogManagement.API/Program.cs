using Delta;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Common;
using WorklogManagement.API.Holidays;
using WorklogManagement.Data.Context;

#if DEBUG
Console.Title = "WorklogManagement.API";
#endif

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);
config.AddJsonFile($"local.settings.{env.EnvironmentName}.json", true);

var attachmentsBaseDir = config.GetValue<string>("AttachmentsBaseDir");
if (!string.IsNullOrWhiteSpace(attachmentsBaseDir))
{
    Configuration.SetAttachmentsBaseDir(attachmentsBaseDir);
}

var isDevelopment = env.IsDevelopment();

var services = builder.Services;

services.AddControllers();

services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "WorklogManagement", Version = "v1" });
});

//services.Configure<SwaggerGenOptions>(options =>
//{
//    options.SwaggerGeneratorOptions.Servers = [
//        new() { Url = "/worklog-management/api" }
//    ];
//});

services.AddHttpClient();

services.AddDbContext<WorklogManagementContext>(options =>
{
    var conStr = config.GetConnectionString("WorklogManagement");

    options.UseSqlServer(conStr);

    //options
    //    .EnableDetailedErrors(isDevelopment)
    //    .EnableSensitiveDataLogging(isDevelopment);
});

var app = builder.Build();

app.UseDelta<WorklogManagementContext>();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapGroup("/health").WithTags("Health").MapGet("", () => Results.Ok());

app.RegisterHolidayEndpoints();

app.Run();
