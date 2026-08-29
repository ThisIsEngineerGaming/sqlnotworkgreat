using System.ComponentModel.DataAnnotations;
using mvc.Models.Validation;

namespace mvc
{
    public class Film
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the film title.")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "The {0} field must be between {2} and {1} characters long.")]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please enter the director's name.")]
        [StringLength(100, ErrorMessage = "The {0} field cannot exceed {1} characters.")]
        [Display(Name = "Director")]
        public string? Director { get; set; }

        // Custom validation attribute — checks that the year is not in the
        // future and not earlier than 1888 (see Models/Validation/NotFutureYearAttribute.cs)
        [NotFutureYear]
        [Display(Name = "Release Year")]
        public int ReleaseYear { get; set; }

        [Required(ErrorMessage = "Please specify the genre.")]
        [StringLength(50, ErrorMessage = "The {0} field cannot exceed {1} characters.")]
        [Display(Name = "Genre")]
        public string? Genre { get; set; }

        // Built-in Range attribute — IMDb-style rating (0 to 10)
        [Range(0, 10, ErrorMessage = "The {0} field must be between {1} and {2}.")]
        [Display(Name = "Rating")]
        public double Rating { get; set; }

        // Custom validation attribute — checks the URL format
        // (see Models/Validation/ValidImageUrlAttribute.cs)
        [ValidImageUrl]
        [Display(Name = "Poster URL")]
        public string? PhotoUrl { get; set; } // URL link to movie poster image
    }
}
