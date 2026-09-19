using mvc.BLL.DTOs;

namespace mvc.BLL.Interfaces;

public interface IFilmService
{
    Task<IReadOnlyList<FilmDto>> GetAllAsync();
    Task<FilmDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(FilmCreateDto dto, PosterUpload? poster);
    Task<bool> UpdateAsync(int id, FilmUpdateDto dto, PosterUpload? poster);
    Task<bool> DeleteAsync(int id);
}
