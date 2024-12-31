using Radzen;
using Serilog;
using WorklogManagement.UI.Components;
using WorklogManagement.UI.Components.Pages.CheckIn;
using WorklogManagement.UI.Components.Pages.Home;
using WorklogManagement.UI.Components.Pages.TicketBoard;
using WorklogManagement.UI.Components.Pages.TicketForm;
using WorklogManagement.UI.Components.Pages.TicketList;
using WorklogManagement.UI.Components.Pages.Tracking;
using WorklogManagement.UI.Components.Pages.WorklogForm;
using WorklogManagement.UI.Services;

#if DEBUG
Console.Title = "WorklogManagement.UI";
#endif

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);

var isDevelopment = builder.Environment.IsDevelopment();

var services = builder.Services;

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

services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

services
    .AddRadzenComponents()
    .AddScoped<ThemeService>()
    .AddScoped<NotificationService>()
    .AddScoped<DialogService>();

services.AddHttpClient(nameof(WorklogManagement), client => client.BaseAddress = new(config.GetValue<string>($"{nameof(WorklogManagement)}{nameof(HttpClient.BaseAddress)}")!));

services
    .AddScoped<CheckInViewModel>()
    .AddScoped<HomeViewModel>()
    .AddScoped<TicketFormViewModel>()
    .AddScoped<TicketBoardViewModel>()
    .AddScoped<TicketListViewModel>()
    .AddScoped<TrackingViewModel>()
    .AddScoped<WorklogFormViewModel>();

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
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
