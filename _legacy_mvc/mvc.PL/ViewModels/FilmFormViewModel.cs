using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace mvc.PL.ViewModels;

public class FilmFormViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Вкажіть назву фільму.")] [StringLength(150)] public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Вкажіть режисера.")] [StringLength(100)] public string Director { get; set; } = string.Empty;
    [Range(1888, 2100, ErrorMessage = "Вкажіть коректний рік.")] public int ReleaseYear { get; set; }
    [Required(ErrorMessage = "Вкажіть жанр.")] [StringLength(50)] public string Genre { get; set; } = string.Empty;
    [Range(0, 10, ErrorMessage = "Рейтинг має бути від 0 до 10.")] public double Rating { get; set; }
    [Url(ErrorMessage = "Вкажіть коректне посилання на постер.")] public string? PhotoUrl { get; set; }
    public IFormFile? Poster { get; set; }
}
