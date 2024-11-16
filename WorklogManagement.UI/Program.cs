using Microsoft.EntityFrameworkCore;
using Radzen;
using WorklogManagement.Data.Context;
using WorklogManagement.Service;
using WorklogManagement.Service.Common;
using WorklogManagement.UI.Components;
using WorklogManagement.UI.ViewModels;

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
    .AddScoped<ThemeService>();

services
    .AddScoped<IWorklogManagementService, WorklogManagementService>()
    .AddDbContext<WorklogManagementContext>(options =>
    {
        var conStr = config.GetConnectionString("WorklogManagement");

        options.UseSqlServer(conStr);

        options
            .EnableDetailedErrors(isDevelopment)
            .EnableSensitiveDataLogging(isDevelopment);
    });

var attachmentsBaseDir = string.IsNullOrWhiteSpace(config.GetValue<string>("AttachmentsBaseDir"))
    ? Path.Combine(".", "Attachments")
    : config.GetValue<string>("AttachmentsBaseDir")!;

Configuration.SetAttachmentsBaseDir(attachmentsBaseDir);

services
    .AddScoped<ChangelogViewModel>()
    .AddScoped<CheckInViewModel>()
    .AddScoped<HomeViewModel>()
    .AddScoped<TicketsViewModel>()
    .AddScoped<TicketViewModel>()
    .AddScoped<TrackingViewModel>()
    .AddScoped<WorklogViewModel>();

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
