using System.Reflection;
using Blazored.LocalStorage;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Radzen;
using Serilog;
using WorklogManagement.UI.Components;
using WorklogManagement.UI.Components.Pages.CheckIn;
using WorklogManagement.UI.Components.Pages.Home;
using WorklogManagement.UI.Components.Pages.TicketBoard;
using WorklogManagement.UI.Components.Pages.TicketForm;
using WorklogManagement.UI.Components.Pages.TicketList;
using WorklogManagement.UI.Components.Pages.Tracking;
using WorklogManagement.UI.Extensions;
using WorklogManagement.UI.Services;

var assemblyVersion = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version?.ToString() ?? string.Empty;

#if DEBUG
Console.Title = $"WorklogManagement.UI {assemblyVersion}";
#endif

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);

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

    services
        .AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>))
        .AddLogging(loggingBuilder =>
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service.name", "worklogmanagement-ui")
            .Enrich.WithProperty("service.version", assemblyVersion)
            .WriteTo.Elasticsearch(
                [esUri],
                opts =>
                {
                    opts.DataStream = new DataStreamName("logs", "worklogmanagement-ui", "prod");
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
    services
        .AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>))
        .AddLogging(loggingBuilder =>
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(logger, dispose: true);
    });
}

services
    .AddRazorComponents()
    .AddInteractiveServerComponents(config.Bind);

services.AddBlazoredLocalStorage();

services
    .AddRadzenComponents()
    .AddScoped<ThemeService>()
    .AddScoped<NotificationService>()
    .AddScoped<DialogService>();

var apiBaseAddress = config.GetValue<string>($"Api{nameof(HttpClient.BaseAddress)}")!;
services.AddHttpClient(nameof(WorklogManagement), client => client.BaseAddress = new(apiBaseAddress));

services
    .AddScoped<CheckInViewModel>()
    .AddScoped<HomeViewModel>()
    .AddScoped<TicketFormViewModel>()
    .AddScoped<TicketBoardViewModel>()
    .AddScoped<TicketListViewModel>()
    .AddScoped<TrackingViewModel>()
    .AddSingleton(new BrandingExtension(assemblyVersion));

services
    .AddSingleton(TimeProvider.System)
    .AddTransient<IDataService, DataService>()
    .AddScoped<IGlobalDataStateService, GlobalDataStateService>()
    .AddTransient<INavigationService, NavigationService>()
    .AddTransient<IPopupService, PopupService>()
    .AddTransient<ITicketStatusService, TicketStatusService>();

var app = builder.Build();

if (isDevelopment)
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UsePathBase(config.GetValue<string>("PathBase"));

app.UseStaticFiles();
app.UseAntiforgery();

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
