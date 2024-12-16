using Radzen;
using WorklogManagement.UI.Components;
using WorklogManagement.UI.Components.Pages.CheckIn;
using WorklogManagement.UI.Components.Pages.Home;
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
    .AddScoped<HomeViewModel>()
    .AddScoped<CheckInViewModel>();

services
    .AddTransient<IDataService, DataService>()
    .AddScoped<IGlobalDataStateService, GlobaleDataStateService>()
    .AddTransient<INavigationService, NavigationService>()
    .AddTransient<IToastService, ToastService>()
    .AddTransient<IPopupService, PopupService>();

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
