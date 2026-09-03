using Microsoft.AspNetCore.Http;
using mvc;

namespace mvc.Services
{
    // Інтерфейс сервісу фільмів — описує всю бізнес-логіку та роботу з БД/файлами,
    // яку раніше виконував FilmsController напряму.
    // Контролер тепер залежить лише від цієї абстракції (Dependency Injection),
    // а не від конкретної реалізації (FilmService) чи від FilmContext/IWebHostEnvironment.
    public interface IFilmService
    {
        // отримати список усіх фільмів
        Task<List<Film>> GetAllFilmsAsync();

        // отримати один фільм за id (або null, якщо не знайдено)
        Task<Film?> GetFilmByIdAsync(int? id);

        // створити новий фільм; photoFile може бути null, якщо постер не завантажується
        Task<Film> CreateFilmAsync(Film film, IFormFile? photoFile);

        // оновити існуючий фільм; повертає false, якщо фільму з таким id не існує (конкурентне видалення)
        Task<bool> UpdateFilmAsync(int id, Film film, IFormFile? photoFile);

        // видалити фільм за id
        Task DeleteFilmAsync(int? id);

        // перевірити, чи існує фільм із заданим id
        bool FilmExists(int? id);
    }
}
