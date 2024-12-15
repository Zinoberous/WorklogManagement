using Delta;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WorklogManagement.API.Common;
using WorklogManagement.Data.Context;

#if DEBUG
Console.Title = "WorklogManagement.API";
#endif

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);

var isDevelopment = builder.Environment.IsDevelopment();

var services = builder.Services;

services.AddLogging(logBuilder =>
{
    var logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .MinimumLevel.Information()
        //.Filter.ByIncludingOnly(e => e.Properties["SourceContext"].ToString().StartsWith($"\"{nameof(WorklogManagement)}"))
        .Filter.ByIncludingOnly(e => e.Properties["SourceContext"].ToString().StartsWith($"\"{nameof(Program)}"))
        .WriteTo.Console()
        .CreateLogger();

    logBuilder.ClearProviders();
    logBuilder.AddSerilog(logger);
});

services.AddControllers();

services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(_ => true);
        builder.AllowCredentials();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

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

var attachmentsBaseDir = config.GetValue<string>("AttachmentsBaseDir");

if (!string.IsNullOrWhiteSpace(attachmentsBaseDir))
{
    Configuration.SetAttachmentsBaseDir(attachmentsBaseDir);
}

var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.ToString().Contains("swagger", StringComparison.InvariantCultureIgnoreCase))
//    {
//        await next.Invoke();
//        return;
//    }

//    var stopwatch = Stopwatch.StartNew();
//    await next.Invoke();
//    stopwatch.Stop();
//    var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
//    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation("Request {Method} {Path} executed in {ElapsedMilliseconds}ms", context.Request.Method, context.Request.Path, elapsedMilliseconds);
//});

app.UseDelta<WorklogManagementContext>();

app.MapControllers();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
