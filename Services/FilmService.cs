using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mvc;
using mvc.Repositories;

namespace mvc.Services
{
    // Реалізація сервісу фільмів.
    // Інтерфейс IFilmService залишився БЕЗ ЗМІН — контролер, як і раніше,
    // працює лише з ним і навіть не підозрює, що всередині щось змінилося.
    //
    // А ось усередині сервіс тепер більше НЕ звертається до FilmContext
    // напряму: замість цього він отримує через DI абстракцію IRepository<Film>
    // і делегує їй усі операції з базою даних. Сам DbContext сервісу більше
    // не потрібен і не інжектується.
    // Це і є принцип інверсії залежностей (DIP): сервіс залежить від
    // абстракції репозиторію, а не від конкретного класу доступу до БД.
    public class FilmService : IFilmService
    {
        private readonly IRepository<Film> _filmRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FilmService(IRepository<Film> filmRepository, IWebHostEnvironment hostEnvironment)
        {
            _filmRepository = filmRepository;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<List<Film>> GetAllFilmsAsync()
        {
            return await _filmRepository.GetAllAsync();
        }

        public async Task<Film?> GetFilmByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return await _filmRepository.GetByIdAsync(id.Value);
        }

        public async Task<Film> CreateFilmAsync(Film film, IFormFile? photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                film.PhotoUrl = await SavePhotoFileAsync(photoFile);
            }

            await _filmRepository.AddAsync(film);
            await _filmRepository.SaveChangesAsync();

            return film;
        }

        public async Task<bool> UpdateFilmAsync(int id, Film film, IFormFile? photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                film.PhotoUrl = await SavePhotoFileAsync(photoFile);
            }

            try
            {
                _filmRepository.Update(film);
                await _filmRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmExists(film.Id))
                {
                    return false;
                }

                throw;
            }

            return true;
        }

        public async Task DeleteFilmAsync(int? id)
        {
            if (id == null)
            {
                return;
            }

            await _filmRepository.DeleteAsync(id.Value);
            await _filmRepository.SaveChangesAsync();
        }

        public bool FilmExists(int? id)
        {
            // Сигнатура методу в IFilmService — синхронна (ми домовилися
            // інтерфейс сервісу не змінювати), а всі операції з БД тепер
            // "заховані" в асинхронному IRepository<T>. Тому тут доводиться
            // синхронно дочекатися результату асинхронного виклику.
            // Викликається лише в рідкісній гілці обробки конфлікту
            // паралельного оновлення (DbUpdateConcurrencyException),
            // тож блокування потоку тут не є проблемою на практиці.
            if (id == null)
            {
                return false;
            }

            return _filmRepository.ExistsAsync(id.Value).GetAwaiter().GetResult();
        }

        // допоміжний приватний метод — інкапсулює роботу з файловою системою
        // (збереження постера фільму в wwwroot/images/films).
        // Це НЕ звернення до бази даних, тому доступ до файлової системи
        // цілком коректно залишається в сервісі, а не в репозиторії.
        private async Task<string> SavePhotoFileAsync(IFormFile photoFile)
        {
            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "films");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photoFile.FileName)}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(fileStream);
            }

            return $"/images/films/{fileName}";
        }
    }
}
