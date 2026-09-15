using mvc.BLL.DTOs;
using mvc.BLL.Exceptions;
using mvc.BLL.Interfaces;

namespace mvc.PL.Services;

public sealed class LocalPosterStorage(IWebHostEnvironment environment) : IPosterStorage
{
    private static readonly HashSet<string> Extensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxBytes = 5 * 1024 * 1024;

    public async Task<string> SaveAsync(PosterUpload poster)
    {
        var extension = Path.GetExtension(poster.FileName).ToLowerInvariant();
        if (!Extensions.Contains(extension) || !poster.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) throw new BusinessRuleException("Постер має бути зображенням JPG, PNG або WebP.");
        if (poster.Length is <= 0 or > MaxBytes) throw new BusinessRuleException("Розмір постера не може перевищувати 5 МБ.");
        var folder = Path.Combine(environment.WebRootPath, "images", "films");
        Directory.CreateDirectory(folder);
        var name = $"{Guid.NewGuid():N}{extension}";
        await using var output = File.Create(Path.Combine(folder, name));
        await poster.Content.CopyToAsync(output);
        return $"/images/films/{name}";
    }

    public Task DeleteAsync(string? relativePath)
    {
        if (!string.IsNullOrWhiteSpace(relativePath) && relativePath.StartsWith("/images/films/", StringComparison.Ordinal) && environment.WebRootPath is { } root)
        {
            var path = Path.Combine(root, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(path)) File.Delete(path);
        }
        return Task.CompletedTask;
    }
}
