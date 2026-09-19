namespace Film.Domain.Entities
{
    public class Film
    {
        public int Id { get; set; } // первинний ключ, ідентифікатор фільму
        public string Title { get; set; } = string.Empty; // назва фільму
        public string Director { get; set; } = string.Empty; // режисер
        public int ReleaseYear { get; set; } // рік випуску
        public string Genre { get; set; } = string.Empty; // жанр
        public double Rating { get; set; } // рейтинг (0-10)
        public string? PhotoUrl { get; set; } // посилання на постер (необов'язкове)
    }
}
