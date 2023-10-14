using DataAccess.WorklogManagement.Context;
using Microsoft.EntityFrameworkCore;

Console.Title = "API.WorklogManagement";

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

config.AddJsonFile("local.settings.json", true);

var services = builder.Services;

services.AddControllers();

services.AddCors
(
    options =>
    {
        options.AddDefaultPolicy
        (
            builder =>
            {
                builder.SetIsOriginAllowed(_ => true);
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            }
        );
    }
);

services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "WorklogManagemen", Version = "v1" });
    }
);

services.AddDbContext<WorklogManagementContext>
(
    options =>
    {
        options.UseSqlServer(config.GetConnectionString("WorklogManagemen")!);
    }
);

var app = builder.Build();

app.MapControllers();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
