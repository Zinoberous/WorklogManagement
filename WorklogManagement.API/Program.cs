using System.Reflection;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using WorklogManagement.API.Absences;
using WorklogManagement.API.Common;
using WorklogManagement.API.Holidays;
using WorklogManagement.API.Statistics;
using WorklogManagement.API.Tickets;
using WorklogManagement.API.Worklogs;
using WorklogManagement.API.WorkTimes;
using WorklogManagement.Data.Context;

var assemblyVersion = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version?.ToString() ?? string.Empty;

#if DEBUG
Console.Title = $"WorklogManagement.API {assemblyVersion}";
#endif

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);

var attachmentsBaseDir = config.GetValue<string>("AttachmentsDir");

if (!string.IsNullOrWhiteSpace(attachmentsBaseDir))
{
    Configuration.SetAttachmentsBaseDir(attachmentsBaseDir);
}

var services = builder.Services;

//// https://github.com/serilog/serilog-sinks-file/issues/56 => RollingInterval.Day mit utc statt local time
//var fileSinkTypes = typeof(Serilog.Sinks.File.FileSink).Assembly.GetTypes();
//var clockType = fileSinkTypes.FirstOrDefault(x => string.Equals(x.FullName, "Serilog.Sinks.File.Clock", StringComparison.Ordinal));
//var timestampProviderField = clockType?.GetField("_dateTimeNow", BindingFlags.Static | BindingFlags.NonPublic);
//timestampProviderField?.SetValue(null, new Func<DateTime>(() => DateTime.UtcNow));

if (builder.Environment.IsProduction() && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ELASTIC_API_KEY")))
{
    var apiKey = Environment.GetEnvironmentVariable("ELASTIC_API_KEY")
        ?? throw new InvalidOperationException("ELASTIC_API_KEY must be provided in Production.");

    var esUri = new Uri("http://localhost:9200");

    services.AddLogging(loggingBuilder =>
    {
        var logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("service.name", "worklogmanagement-api")
        .Enrich.WithProperty("service.version", assemblyVersion)
        .WriteTo.Elasticsearch(
            [esUri],
            opts =>
            {
                opts.DataStream = new DataStreamName("logs", "worklogmanagement-api", "prod");
                opts.BootstrapMethod = BootstrapMethod.None;
            },
            transport => transport.Authentication(new ApiKey(apiKey!))
        )
        .CreateLogger();

        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(logger, dispose: true);
    });
}
else
{
    services.AddLogging(loggingBuilder =>
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(logger, dispose: true);
    });
}

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
    options.SwaggerDoc("v1", new() { Title = $"WorklogManagement {assemblyVersion}", Version = "v1" });
    options.CustomSchemaIds(type => type.FullName);
});


if (!isDevelopment)
{
    services.Configure<SwaggerGenOptions>(options =>
    {
        options.SwaggerGeneratorOptions.Servers = [
            new() { Url = config.GetValue<string>("PathBase") }
        ];
    });
}

services.AddHttpClient();

var conStr = !string.IsNullOrEmpty(config.GetConnectionString("WorklogManagement"))
        ? config.GetConnectionString("WorklogManagement")
        : throw new NotImplementedException("Kein Connectionstring für 'WorklogManagement' angegeben.");

services.AddDbContext<WorklogManagementContext>(options =>
{
    options.UseSqlServer(conStr);
});

services.AddHealthChecks()
    .AddDbContextCheck<WorklogManagementContext>();

var app = builder.Build();

app.UseCors();

app.UsePathBase(config.GetValue<string>("PathBase"));


if (!isDevelopment)
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        var serverUrl = $"https://{httpReq.Host.Value}{httpReq.PathBase}";
        swaggerDoc.Servers = [
            new() { Url = serverUrl }
        ];
    });
});

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
app.MapHealthChecks("/healthcheck");

app.RegisterHolidayEndpoints();
app.RegisterStatisticEndpoints();
app.RegisterWorkTimeEndpoints();
app.RegisterAbsenceEndpoints();
app.RegisterTicketEndpoints();
app.RegisterWorklogEndpoints();

app.Run();
