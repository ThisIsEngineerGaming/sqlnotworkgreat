using mvc.Repositories;
using mvc.Services;

namespace mvc.Extensions
{
    // Extension-метод для IServiceCollection.
    // Замість того, щоб у Program.cs напряму викликати
    // builder.Services.AddScoped<IFilmService, FilmService>();
    // builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    // ми ховаємо цю деталь реєстрації всередину одного зрозумілого методу.
    public static class ServiceCollectionExtensions
    {
        // AddApplicationServices — власний extension-метод, що додає
        // всі сервіси прикладного (бізнес) рівня застосунку,
        // а також шар репозиторіїв, через який єдиний відбувається
        // доступ до бази даних (FilmContext).
        // Викликається в Program.cs як звичайний "рідний" метод: builder.Services.AddApplicationServices();
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Scoped — новий екземпляр на кожен HTTP-запит,
            // так само як і FilmContext (EF Core DbContext теж Scoped за замовчуванням).

            // реєструємо узагальнений репозиторій: будь-яка залежність
            // від IRepository<T> (наприклад, IRepository<Film>) буде задоволена
            // реалізацією Repository<T>, яка всередині працює з FilmContext.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // реєструємо сервісний шар: FilmsController залежить від IFilmService,
            // а сама реалізація FilmService тепер, у свою чергу, залежить
            // від IRepository<Film> — а не від FilmContext напряму.
            services.AddScoped<IFilmService, FilmService>();

            return services;
        }
    }
}
