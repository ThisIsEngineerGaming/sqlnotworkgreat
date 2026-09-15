using Microsoft.AspNetCore.Mvc;
using mvc.BLL.DTOs;
using mvc.BLL.Exceptions;
using mvc.BLL.Interfaces;
using mvc.PL.ViewModels;

namespace mvc.PL.Controllers;

public class FilmsController(IFilmService filmService) : Controller
{
    public async Task<IActionResult> Index() => View(await filmService.GetAllAsync());

    public async Task<IActionResult> Details(int id) => (await filmService.GetByIdAsync(id)) is { } film ? View(film) : NotFound();

    public IActionResult Create() => View(new FilmFormViewModel { ReleaseYear = DateTime.UtcNow.Year });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FilmFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var id = await filmService.CreateAsync(ToCreateDto(model), ToUpload(model.Poster));
            TempData["SuccessMessage"] = "Фільм успішно створено.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (BusinessRuleException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    public async Task<IActionResult> Edit(int id) => (await filmService.GetByIdAsync(id)) is { } film ? View(ToFormModel(film)) : NotFound();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FilmFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        try
        {
            if (!await filmService.UpdateAsync(id, ToUpdateDto(model), ToUpload(model.Poster))) return NotFound();
            TempData["SuccessMessage"] = "Фільм успішно оновлено.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (BusinessRuleException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    public async Task<IActionResult> Delete(int id) => (await filmService.GetByIdAsync(id)) is { } film ? View(film) : NotFound();

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await filmService.DeleteAsync(id)) return NotFound();
        TempData["SuccessMessage"] = "Фільм успішно видалено.";
        return RedirectToAction(nameof(Index));
    }

    private static FilmCreateDto ToCreateDto(FilmFormViewModel m) => new() { Title = m.Title, Director = m.Director, ReleaseYear = m.ReleaseYear, Genre = m.Genre, Rating = m.Rating, PhotoUrl = m.PhotoUrl };
    private static FilmUpdateDto ToUpdateDto(FilmFormViewModel m) => new() { Title = m.Title, Director = m.Director, ReleaseYear = m.ReleaseYear, Genre = m.Genre, Rating = m.Rating, PhotoUrl = m.PhotoUrl };
    private static FilmFormViewModel ToFormModel(FilmDto f) => new() { Id = f.Id, Title = f.Title, Director = f.Director, ReleaseYear = f.ReleaseYear, Genre = f.Genre, Rating = f.Rating, PhotoUrl = f.PhotoUrl };
    private static PosterUpload? ToUpload(IFormFile? file) => file is { Length: > 0 } ? new PosterUpload(file.OpenReadStream(), file.FileName, file.ContentType, file.Length) : null;
}
