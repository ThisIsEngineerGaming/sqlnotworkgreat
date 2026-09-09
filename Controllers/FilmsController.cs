using Microsoft.AspNetCore.Mvc;
using mvc;
using mvc.Services;

// Контролер перероблено на Web API: більше не повертає Views,
// а віддає дані у форматі JSON. Уся бізнес-логіка та доступ
// до даних, як і раніше, винесені в сервісний шар (Services/IFilmService, Services/FilmService),
// контролер лише відповідає за HTTP-частину (маршрути, статус-коди, серіалізація).
[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly IFilmService _filmService;

    public FilmsController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    // GET: api/Films
    [HttpGet]
    public async Task<ActionResult<List<Film>>> GetAll()
    {
        return Ok(await _filmService.GetAllFilmsAsync());
    }

    // GET: api/Films/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Film>> GetById(int id)
    {
        var film = await _filmService.GetFilmByIdAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        return Ok(film);
    }

    // POST: api/Films
    // multipart/form-data: поля Film + необов'язковий файл PhotoFile
    [HttpPost]
    public async Task<ActionResult<Film>> Create([FromForm] Film film, IFormFile? photoFile)
    {
        try
        {
            var created = await _filmService.CreateFilmAsync(film, photoFile);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("photoFile", "Error uploading file: " + ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    // PUT: api/Films/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] Film film, IFormFile? photoFile)
    {
        if (id != film.Id)
        {
            return BadRequest("Route id and film id do not match.");
        }

        try
        {
            bool updated = await _filmService.UpdateFilmAsync(id, film, photoFile);
            if (!updated)
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("photoFile", "Error uploading file: " + ex.Message);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }

    // DELETE: api/Films/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var film = await _filmService.GetFilmByIdAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        await _filmService.DeleteFilmAsync(id);
        return NoContent();
    }
}
