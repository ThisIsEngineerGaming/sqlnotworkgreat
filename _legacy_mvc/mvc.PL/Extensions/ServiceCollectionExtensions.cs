using mvc.BLL.Interfaces;
using mvc.BLL.Services;
using mvc.DAL;
using mvc.DAL.Repositories;
using mvc.PL.Services;

namespace mvc.PL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPosterStorage, LocalPosterStorage>();
        services.AddScoped<IFilmService, FilmService>();
        return services;
    }
}
