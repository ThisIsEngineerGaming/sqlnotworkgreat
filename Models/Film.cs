namespace mvc
{
    public class Film
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Director { get; set; }
        public int ReleaseYear { get; set; }
        public string? Genre { get; set; }
        public double Rating { get; set; } // IMDb-style rating (0-10)
        public string? PhotoUrl { get; set; } // URL link to movie poster image
    }
}
