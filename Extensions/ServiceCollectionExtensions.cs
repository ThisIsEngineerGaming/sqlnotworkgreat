using mvc.Services;

namespace mvc.Extensions
{
    // Extension-метод для IServiceCollection.
    // Замість того, щоб у Program.cs напряму викликати
    // builder.Services.AddScoped<IFilmService, FilmService>();
    // ми ховаємо цю деталь реєстрації всередину одного зрозумілого методу.
    // Це той самий підхід, що і в прикладі
    // https://github.com/sunmeat/aspnetcore_services —
    // сервіси реєструються через власний extension-метод над IServiceCollection.
    public static class ServiceCollectionExtensions
    {
        // AddApplicationServices — власний extension-метод, що додає
        // всі сервіси прикладного (бізнес) рівня застосунку.
        // Викликається в Program.cs як звичайний "рідний" метод: builder.Services.AddApplicationServices();
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Scoped — новий екземпляр сервісу на кожен HTTP-запит,
            // так само як і FilmContext (EF Core DbContext теж Scoped за замовчуванням)
            services.AddScoped<IFilmService, FilmService>();

            return services;
        }
    }
}
