using AutoMapper;
using Film.Application.DTO;

namespace Film.Application.Mapping
{
    /// <summary>
    /// Єдиний AutoMapper-профіль, зареєстрований один раз через AddAutoMapper
    /// у DI-контейнері (Application/DependencyInjection). IMapper інжектиться
    /// в FilmService через конструктор.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.Film, FilmDTO>(); // мепінг простий, тому що властивості збігаються за назвою та типами
        }
    }
}
