using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mvc;

namespace mvc.Services
{
    // Реалізація сервісу фільмів.
    // Сюди перенесено всю бізнес-логіку та роботу з файлами/БД,
    // яка раніше "жила" прямо в FilmsController.
    // Сервіс отримує свої залежності (FilmContext, IWebHostEnvironment)
    // через конструктор — так само за допомогою Dependency Injection,
    // тільки тепер вони інкапсульовані тут, а не в контролері.
    public class FilmService : IFilmService
    {
        private readonly FilmContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FilmService(FilmContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<List<Film>> GetAllFilmsAsync()
        {
            return await _context.Films.ToListAsync();
        }

        public async Task<Film?> GetFilmByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return await _context.Films.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Film> CreateFilmAsync(Film film, IFormFile? photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                film.PhotoUrl = await SavePhotoFileAsync(photoFile);
            }

            _context.Add(film);
            await _context.SaveChangesAsync();

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
                _context.Update(film);
                await _context.SaveChangesAsync();
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
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                _context.Films.Remove(film);
            }

            await _context.SaveChangesAsync();
        }

        public bool FilmExists(int? id)
        {
            return _context.Films.Any(e => e.Id == id);
        }

        // допоміжний приватний метод — інкапсулює роботу з файловою системою
        // (збереження постера фільму в wwwroot/images/films)
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
