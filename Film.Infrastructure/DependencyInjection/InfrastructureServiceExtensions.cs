using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Film.Domain.Interfaces;
using Film.Infrastructure.Persistence;
using Film.Infrastructure.Repositories;

namespace Film.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Уся "технічна" реєстрація живе тут: DbContext, підключення до Postgres,
    /// репозиторій, Unit of Work. Presentation (Film.WebAPI, композиційний корінь)
    /// лише викликає AddInfrastructure(...) у Program.cs.
    /// </summary>
    public static class InfrastructureServiceExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, string? connection)
        {
            services.AddDbContext<FilmContext>(options =>
                options.UseNpgsql(connection)); // реєструємо контекст для роботи з Postgres

            services.AddScoped<IUnitOfWork, EFUnitOfWork>(); // реєструємо unit of work з реалізацією на entity framework
        }
    }
}
