namespace mvc.BLL.DTOs;

public class FilmCreateDto
{
    public string Title { get; init; } = string.Empty;
    public string Director { get; init; } = string.Empty;
    public int ReleaseYear { get; init; }
    public string Genre { get; init; } = string.Empty;
    public double Rating { get; init; }
    public string? PhotoUrl { get; init; }
}
