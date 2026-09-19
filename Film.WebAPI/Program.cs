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

var app = builder.Build();

app.MapControllers();

app.Run();
