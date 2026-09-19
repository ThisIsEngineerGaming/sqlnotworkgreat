using Microsoft.EntityFrameworkCore;
using mvc.DAL;
using mvc.PL.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FilmContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationLayers();

var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Films/Index");
app.UseStaticFiles();
app.UseRouting();
using (var scope = app.Services.CreateScope()) scope.ServiceProvider.GetRequiredService<FilmContext>().EnsureDatabaseCreatedAndSeeded();
app.MapControllerRoute(name: "default", pattern: "{controller=Films}/{action=Index}/{id?}");
app.Run();
