using Film.Application.DependencyInjection;
using Film.Infrastructure.DependencyInjection;

// View > Terminal:
// cd film.client
// npm install

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddInfrastructure(connection);
builder.Services.AddApplication();

builder.Services.AddControllers();

// CORS: дозволяє React-застосунок з іншого домену (Render Static Site) викликати API.
// Задайте Cors__AllowedOrigins (адреси через кому) у змінних середовища; якщо не задано - дозволено всі джерела.
var allowedOrigins = builder.Configuration
    .GetValue<string>("Cors:AllowedOrigins")?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins is { Length: > 0 })
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.MapControllers();

app.Run();
