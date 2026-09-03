using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mvc.Extensions;
// dotnet add package Microsoft.EntityFrameworkCore !!! встановлюємо пакети, інакше код не працюватиме, View > Terminal
// dotnet add package Microsoft.EntityFrameworkCore.SqlServer !!!

namespace mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // отримуємо рядок підключення з конфігураційного файлу appsettings.json (ідемо туди і дивимося)
            // звісно, має бути піднятий SQL Server, перевірити дані для підключення можна в SQL Server Management Studio
            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // додаємо контекст бази даних (дивимось Student.cs та StudentContext.cs)
            builder.Services.AddDbContext<FilmContext>(options => options.UseNpgsql(connection));
            // на основі рядка вище, інфраструктура ASP.NET Core створить об'єкт StudentContext, і передасть його в контролер StudentController через механізм впровадження залежностей (Dependency Injection)
            // до речі, в старому ASP.NET (не Core) такого не було, там треба було самому створювати об'єкти контексту даних, а тут все робиться автоматично (в Spring Boot для Java теж так само)
            // !!! два рядки коду вище будуть потрібні завжди, коли треба підключитися до бази даних через Entity Framework !!!

            // додаємо сервіси MVC, інакше не працюватимуть контролери і не підтягнуться вью
            builder.Services.AddControllersWithViews();

            // реєструємо власний сервісний шар (IFilmService -> FilmService) через extension-метод,
            // винесений в Extensions/ServiceCollectionExtensions.cs — саме сюди тепер звертатиметься
            // FilmsController через Dependency Injection, а не напряму до FilmContext
            builder.Services.AddApplicationServices();

            var app = builder.Build();

            // перевіряємо, чи існує база даних Postgres, і якщо ні — створюємо її (один раз, при старті)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FilmContext>();
                db.EnsureDatabaseCreatedAndSeeded();
            }

            // обслуговуємо статичні файли з wwwroot
            app.UseStaticFiles();

            // стандартний маршрут, при заході на корінь сайту відкривається метод Index контролера Films
            // при бажанні, можна буде вказати айді фільму в адресі, наприклад: /Films/Index/2
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Films}/{action=Index}/{id?}"); // тут Films - це назва саме контролера, а не моделі
            // насправді, клас контролера називається FilmsController, але в маршруті вказується лише Films, цього достатньо тому що фреймворк сам додасть слово Controller, є таке правило
            // можна легко замінити контролер на будь-який інший, наприклад Home, тоді відкриватиметься HomeController.cs
            // роутів може бути багато, вони перевірятимуться зверху вниз

            // дивимось клас контролера FilmsController.cs та метод Index там

            app.Run(); // запускаємо веб-додаток
        }
    }
}