using AutoMapper;
using Film.Application.DTO;
using Film.Application.Interfaces;
using Film.Common.Exceptions;
using Film.Domain.Interfaces;

namespace Film.Application.Services
{
    public class FilmService : IEntityService<FilmDTO>
    {
        private IUnitOfWork Database { get; set; } // юніт оф ворк для доступу до репозиторію фільмів
        private readonly IMapper mapper;

        public FilmService(IUnitOfWork uow, IMapper mapper)
        {
            Database = uow;
            this.mapper = mapper;
        }

        public async Task Create(FilmDTO filmDto)
        {
            await ValidateAsync(filmDto, null); // перевіряємо бізнес-правила (дублікат назви тощо)

            var film = new Domain.Entities.Film // приклад ручного мапінгу DTO в Entity
            {
                Id = filmDto.Id,
                Title = filmDto.Title.Trim(),
                Director = filmDto.Director.Trim(),
                ReleaseYear = filmDto.ReleaseYear,
                Genre = filmDto.Genre.Trim(),
                Rating = filmDto.Rating,
                PhotoUrl = filmDto.PhotoUrl
            };

            await Database.Films.Create(film); // створюємо сутність фільму
            await Database.Save(); // зберігаємо зміни в БД
        }

        public async Task Update(FilmDTO filmDto)
        {
            await ValidateAsync(filmDto, filmDto.Id); // перевіряємо бізнес-правила, виключаючи поточний запис із перевірки дублікатів

            var film = new Domain.Entities.Film
            {
                Id = filmDto.Id,
                Title = filmDto.Title.Trim(),
                Director = filmDto.Director.Trim(),
                ReleaseYear = filmDto.ReleaseYear,
                Genre = filmDto.Genre.Trim(),
                Rating = filmDto.Rating,
                PhotoUrl = filmDto.PhotoUrl
            };

            Database.Films.Update(film); // оновлюємо сутність
            await Database.Save(); // зберігаємо зміни
        }

        public async Task Delete(int id)
        {
            await Database.Films.Delete(id); // видаляємо фільм за ідентифікатором
            await Database.Save(); // зберігаємо зміни
        }

        public async Task<FilmDTO> Get(int id)
        {
            var film = await Database.Films.Get(id); // отримуємо фільм за ідентифікатором із БД
            if (film == null)
                throw new ValidationException("Немає такого фільму!"); // викидаємо виключення, якщо фільм не знайдено

            return mapper.Map<FilmDTO>(film);
        }

        // automapper дозволяє проєціювати одну модель на іншу, що зменшує обсяг коду та спрощує програму
        public async Task<IEnumerable<FilmDTO>> GetAll()
        {
            var films = (await Database.Films.GetAll()).OrderBy(f => f.Title); // сортуємо за назвою, як у вихідному MVC-сервісі
            return mapper.Map<IEnumerable<FilmDTO>>(films);
        }

        // перенесено з mvc.BLL.Services.FilmService: бізнес-правила, які не є простою анотацією моделі
        private async Task ValidateAsync(FilmDTO dto, int? currentId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new BusinessRuleException("Назва фільму є обов'язковою.");
            if (string.IsNullOrWhiteSpace(dto.Director) || string.IsNullOrWhiteSpace(dto.Genre))
                throw new BusinessRuleException("Вкажіть режисера та жанр.");
            if (dto.ReleaseYear is < 1888 or > 2100 || dto.ReleaseYear > DateTime.UtcNow.Year)
                throw new BusinessRuleException("Вкажіть коректний рік випуску.");
            if (dto.Rating is < 0 or > 10)
                throw new BusinessRuleException("Рейтинг має бути від 0 до 10.");

            var duplicate = (await Database.Films.GetAll())
                .Any(f => f.Id != currentId && string.Equals(f.Title, dto.Title.Trim(), StringComparison.OrdinalIgnoreCase));
            if (duplicate)
                throw new BusinessRuleException("Фільм із такою назвою вже існує.");
        }
    }
}
