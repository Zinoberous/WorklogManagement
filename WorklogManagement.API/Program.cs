using Delta;
using Microsoft.EntityFrameworkCore;
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

var env = builder.Environment;

var isDevelopment = env.IsDevelopment();

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);

var attachmentsBaseDir = config.GetValue<string>("AttachmentsBaseDir");
if (!string.IsNullOrWhiteSpace(attachmentsBaseDir))
{
    Configuration.SetAttachmentsBaseDir(attachmentsBaseDir);
}

var services = builder.Services;

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

app.MapGroup("/health").WithTags("Health").MapGet("", () => Results.Ok());

app.RegisterHolidayEndpoints();
app.RegisterStatisticEndpoints();
app.RegisterWorkTimeEndpoints();
app.RegisterAbsenceEndpoints();
app.RegisterTicketEndpoints();
app.RegisterWorklogEndpoints();

app.Run();
