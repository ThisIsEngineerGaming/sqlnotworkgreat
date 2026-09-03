using Microsoft.AspNetCore.Mvc;
using mvc;
using mvc.Services;

// Контролер тепер "тонкий" (thin controller): жодної роботи з FilmContext
// чи файловою системою тут більше немає. Уся бізнес-логіка та доступ
// до даних винесені в сервісний шар (Services/IFilmService, Services/FilmService).
// Контролер лише отримує IFilmService через Dependency Injection
// і викликає його методи, а сам відповідає тільки за HTTP/View-частину.
public class FilmsController : Controller
{
    private readonly IFilmService _filmService;

    public FilmsController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    // GET: FILMS
    public async Task<IActionResult> Index()
    {
        return View(await _filmService.GetAllFilmsAsync());
    }

    // GET: FILMS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        var film = await _filmService.GetFilmByIdAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        return View(film);
    }

    // GET: FILMS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: FILMS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Director,ReleaseYear,Genre,Rating,PhotoUrl")] Film film, IFormFile PhotoFile)
    {
        if (!ModelState.IsValid)
        {
            return View(film);
        }

        try
        {
            await _filmService.CreateFilmAsync(film, PhotoFile);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("PhotoFile", "Error uploading file: " + ex.Message);
            return View(film);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: FILMS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        var film = await _filmService.GetFilmByIdAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        return View(film);
    }

    // POST: FILMS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Director,ReleaseYear,Genre,Rating,PhotoUrl")] Film film, IFormFile PhotoFile)
    {
        if (id != film.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(film);
        }

        try
        {
            bool updated = await _filmService.UpdateFilmAsync(id.Value, film, PhotoFile);
            if (!updated)
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("PhotoFile", "Error uploading file: " + ex.Message);
            return View(film);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: FILMS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        var film = await _filmService.GetFilmByIdAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        return View(film);
    }

    // POST: FILMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        await _filmService.DeleteFilmAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
