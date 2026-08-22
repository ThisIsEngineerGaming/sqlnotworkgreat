using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc;

public class FilmsController : Controller
{
    private readonly FilmContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public FilmsController(FilmContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    // GET: FILMS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Films.ToListAsync());
    }

    // GET: FILMS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var film = await _context.Films
            .FirstOrDefaultAsync(m => m.Id == id);
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
        if (ModelState.IsValid)
        {
            // Handle file upload if provided
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "films");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(PhotoFile.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await PhotoFile.CopyToAsync(fileStream);
                    }

                    film.PhotoUrl = $"/images/films/{fileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("PhotoFile", "Error uploading file: " + ex.Message);
                    return View(film);
                }
            }

            _context.Add(film);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(film);
    }

    // GET: FILMS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var film = await _context.Films.FindAsync(id);
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

        if (ModelState.IsValid)
        {
            try
            {
                // Handle file upload if provided
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    try
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "films");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(PhotoFile.FileName)}";
                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await PhotoFile.CopyToAsync(fileStream);
                        }

                        film.PhotoUrl = $"/images/films/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("PhotoFile", "Error uploading file: " + ex.Message);
                        return View(film);
                    }
                }

                _context.Update(film);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmExists(film.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(film);
    }

    // GET: FILMS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var film = await _context.Films
            .FirstOrDefaultAsync(m => m.Id == id);
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
        var film = await _context.Films.FindAsync(id);
        if (film != null)
        {
            _context.Films.Remove(film);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FilmExists(int? id)
    {
        return _context.Films.Any(e => e.Id == id);
    }
}
