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
using WorklogManagement.Shared;

#if DEBUG
Console.Title = $"WorklogManagement.API {Assembly.Version}";
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

services.AddLogging(loggingBuilder =>
{
    var loggerConfig = new LoggerConfiguration()
        .ReadFrom.Configuration(config);

    var logger = loggerConfig.CreateLogger();

    if (builder.Environment.IsProduction() && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ELASTIC_API_KEY")))
    {
        var esUri = new Uri("http://localhost:9200");
        var apiKey = Environment.GetEnvironmentVariable("ELASTIC_API_KEY");

        loggerConfig.WriteTo.Elasticsearch(
            [esUri],
            opts =>
            {
                // type = "logs", dataset = "worklogmanagement-api", namespace = "prod"
                opts.DataStream = new DataStreamName("logs", "worklogmanagement-api", "prod");
                opts.BootstrapMethod = BootstrapMethod.Silent;
            },
            transport =>
            {
                transport.Authentication(new ApiKey(
                    !string.IsNullOrEmpty(apiKey)
                        ? apiKey
                        : throw new NotImplementedException("apiKey musn't be empty or null")
                ));
            });
    }

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
    options.SwaggerDoc("v1", new() { Title = $"WorklogManagement {Assembly.Version}", Version = "v1" });
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

var conStr = config.GetConnectionString("WorklogManagement");
if (string.IsNullOrEmpty(conStr))
{
    throw new NotImplementedException("Kein ConnectionString für 'WorklogManagement' angegeben.");
}

services
    .AddDbContext<WorklogManagementContext>(options => options.UseSqlServer(conStr))
    .AddHealthChecks()
    .AddDbContextCheck<WorklogManagementContext>(
        name: "dbcontext",
        tags: ["db", "sql", "ef"],
        customTestQuery: async (ctx, ct) =>
        {
            await ctx.Database.ExecuteSqlRawAsync("SELECT 1", ct);
            return true; // wird bei Exception nicht erreicht
        });

var app = builder.Build();

app.UseCors();

app.UsePathBase(config.GetValue<string>("PathBase"));
app.UseHsts();

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

app.MapHealthChecks("/health");

app.RegisterHolidayEndpoints();
app.RegisterStatisticEndpoints();
app.RegisterWorkTimeEndpoints();
app.RegisterAbsenceEndpoints();
app.RegisterTicketEndpoints();
app.RegisterWorklogEndpoints();

app.Run();
