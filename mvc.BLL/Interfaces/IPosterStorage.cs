using mvc.BLL.DTOs;

namespace mvc.BLL.Interfaces;

public interface IPosterStorage
{
    Task<string> SaveAsync(PosterUpload poster);
    Task DeleteAsync(string? relativePath);
}
