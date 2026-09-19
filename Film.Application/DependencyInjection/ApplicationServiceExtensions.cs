using Film.Application.DTO;
using Film.Application.Interfaces;
using Film.Application.Mapping;
using Film.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Film.Application.DependencyInjection
{
    /// <summary>
    /// Реєструє все, що належить до шару Application: AutoMapper-профіль
    /// та сервіс use-case'ів (FilmService) за його інтерфейсом.
    /// Presentation викликає лише цей один метод у Program.cs, не знаючи деталей.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            // сканує складання Film.Application (за маркерним типом MappingProfile)
            // і реєструє всі знайдені в ньому AutoMapper-профілі
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            services.AddTransient<IEntityService<FilmDTO>, FilmService>(); // реєструємо сервіс фільмів
        }
    }
}
