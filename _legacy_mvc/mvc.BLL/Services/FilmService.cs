using mvc.BLL.DTOs;
using mvc.BLL.Exceptions;
using mvc.BLL.Interfaces;
using mvc.DAL;
using mvc.DAL.Entities;

namespace mvc.BLL.Services;

public sealed class FilmService(IUnitOfWork unitOfWork, IPosterStorage posterStorage) : IFilmService
{
    public async Task<IReadOnlyList<FilmDto>> GetAllAsync() => (await unitOfWork.Films.GetAllAsync()).OrderBy(f => f.Title).Select(ToDto).ToList();
    public async Task<FilmDto?> GetByIdAsync(int id) => (await unitOfWork.Films.GetByIdAsync(id)) is { } film ? ToDto(film) : null;

    public async Task<int> CreateAsync(FilmCreateDto dto, PosterUpload? poster)
    {
        await ValidateAsync(dto, null);
        var film = ToEntity(dto);
        if (poster is not null) film.PhotoUrl = await posterStorage.SaveAsync(poster);
        await unitOfWork.Films.AddAsync(film);
        await unitOfWork.SaveChangesAsync();
        return film.Id;
    }

    public async Task<bool> UpdateAsync(int id, FilmUpdateDto dto, PosterUpload? poster)
    {
        var film = await unitOfWork.Films.GetByIdAsync(id);
        if (film is null) return false;
        await ValidateAsync(dto, id);
        var previousPoster = film.PhotoUrl;
        film.Title = dto.Title.Trim(); film.Director = dto.Director.Trim(); film.Genre = dto.Genre.Trim();
        film.ReleaseYear = dto.ReleaseYear; film.Rating = dto.Rating; film.PhotoUrl = dto.PhotoUrl;
        if (poster is not null) film.PhotoUrl = await posterStorage.SaveAsync(poster);
        unitOfWork.Films.Update(film);
        await unitOfWork.SaveChangesAsync();
        if (poster is not null && previousPoster != film.PhotoUrl) await posterStorage.DeleteAsync(previousPoster);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var film = await unitOfWork.Films.GetByIdAsync(id);
        if (film is null) return false;
        unitOfWork.Films.Remove(film);
        await unitOfWork.SaveChangesAsync();
        await posterStorage.DeleteAsync(film.PhotoUrl);
        return true;
    }

    private async Task ValidateAsync(FilmCreateDto dto, int? currentId)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) throw new BusinessRuleException("Назва фільму є обов’язковою.");
        if (string.IsNullOrWhiteSpace(dto.Director) || string.IsNullOrWhiteSpace(dto.Genre)) throw new BusinessRuleException("Вкажіть режисера та жанр.");
        if (dto.ReleaseYear is < 1888 or > 2100 || dto.ReleaseYear > DateTime.UtcNow.Year) throw new BusinessRuleException("Вкажіть коректний рік випуску.");
        if (dto.Rating is < 0 or > 10) throw new BusinessRuleException("Рейтинг має бути від 0 до 10.");
        if ((await unitOfWork.Films.GetAllAsync()).Any(f => f.Id != currentId && string.Equals(f.Title, dto.Title.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new BusinessRuleException("Фільм із такою назвою вже існує.");
    }

    private static Film ToEntity(FilmCreateDto d) => new() { Title = d.Title.Trim(), Director = d.Director.Trim(), Genre = d.Genre.Trim(), ReleaseYear = d.ReleaseYear, Rating = d.Rating, PhotoUrl = d.PhotoUrl };
    private static FilmDto ToDto(Film f) => new() { Id = f.Id, Title = f.Title, Director = f.Director, Genre = f.Genre, ReleaseYear = f.ReleaseYear, Rating = f.Rating, PhotoUrl = f.PhotoUrl };
}
