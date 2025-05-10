using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WorklogManagement.API.Absences;
using WorklogManagement.API.Common;
using WorklogManagement.API.Holidays;
using WorklogManagement.API.Statistics;
using WorklogManagement.API.Tickets;
using WorklogManagement.API.Worklogs;
using WorklogManagement.API.WorkTimes;
using WorklogManagement.Data.Context;

#if DEBUG
Console.Title = "WorklogManagement.API";
#endif

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);

var attachmentsBaseDir = config.GetValue<string>("AttachmentsBaseDir");
if (!string.IsNullOrWhiteSpace(attachmentsBaseDir))
{
    Configuration.SetAttachmentsBaseDir(attachmentsBaseDir);
}

var services = builder.Services;

// https://github.com/serilog/serilog-sinks-file/issues/56 => RollingInterval.Day mit utc statt local time
var fileSinkTypes = typeof(Serilog.Sinks.File.FileSink).Assembly.GetTypes();
var clockType = fileSinkTypes.FirstOrDefault(x => string.Equals(x.FullName, "Serilog.Sinks.File.Clock", StringComparison.Ordinal));
var timestampProviderField = clockType?.GetField("_dateTimeNow", BindingFlags.Static | BindingFlags.NonPublic);
timestampProviderField?.SetValue(null, new Func<DateTime>(() => DateTime.UtcNow));

services.AddLogging(loggingBuilder =>
{
    var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .CreateLogger();

    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(logger, dispose: true);
});

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
    options.CustomSchemaIds(type => type.FullName);
});

services.Configure<SwaggerGenOptions>(options =>
{
    options.SwaggerGeneratorOptions.Servers = [
        new() { Url = "/stage-worklog-management/api" }
    ];
});

services.AddHttpClient();

services.AddDbContext<WorklogManagementContext>(options =>
{
    var conStr = config.GetConnectionString("WorklogManagement");
    options.UseSqlServer(conStr);
});

var app = builder.Build();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    var queryString = context.Request.QueryString.HasValue
        ? Uri.UnescapeDataString(context.Request.QueryString.Value)
        : string.Empty;

    var api = $"{context.Request.Path}{queryString}";

    string? requestBody = null;

    context.Request.EnableBuffering(); // Ermöglicht mehrfaches Lesen des Anfrage-Bodys
    if (context.Request.ContentLength > 0)
    {
        using StreamReader reader = new(context.Request.Body, leaveOpen: true);
        requestBody = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
    }

    if (string.IsNullOrWhiteSpace(requestBody))
    {
        logger.LogInformation("Anfrage {Method} {API}", context.Request.Method, api);
    }
    else
    {
        logger.LogInformation("Anfrage {Method} {API} {@Content}", context.Request.Method, api, requestBody);
    }

    var originalBodyStream = context.Response.Body;
    using MemoryStream resBodyStream = new();
    context.Response.Body = resBodyStream;

    try
    {
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Antwort {Method} {API} {StatusCode}", context.Request.Method, api, context.Response.StatusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.Body = originalBodyStream;

        await context.Response.WriteAsJsonAsync(ex.Message);

        return;
    }

    resBodyStream.Seek(0, SeekOrigin.Begin);
    var resBody = await new StreamReader(resBodyStream).ReadToEndAsync();
    resBodyStream.Seek(0, SeekOrigin.Begin);

    if (string.IsNullOrWhiteSpace(resBody))
    {
        logger.LogInformation("Antwort {Method} {API} {StatusCode}", context.Request.Method, api, context.Response.StatusCode);
    }
    else
    {
        logger.LogInformation("Antwort {Method} {API} {StatusCode} {@Content}", context.Request.Method, api, context.Response.StatusCode, resBody);
    }

    await resBodyStream.CopyToAsync(originalBodyStream);
});

app.MapGroup("/health").WithTags("Health").MapGet("", () => Results.Ok());

app.RegisterHolidayEndpoints();
app.RegisterStatisticEndpoints();
app.RegisterWorkTimeEndpoints();
app.RegisterAbsenceEndpoints();
app.RegisterTicketEndpoints();
app.RegisterWorklogEndpoints();

app.Run();
