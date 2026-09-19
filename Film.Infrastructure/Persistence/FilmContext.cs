using Microsoft.EntityFrameworkCore;
using Film.Domain.Entities;

namespace Film.Infrastructure.Persistence
{
    public class FilmContext : DbContext
    {
        public FilmContext(DbContextOptions<FilmContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Domain.Entities.Film> Films { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Film>()
                .HasIndex(f => f.Title)
                .IsUnique();

            // перенесені початкові дані з mvc.DAL.FilmContext.EnsureDatabaseCreatedAndSeeded
            modelBuilder.Entity<Domain.Entities.Film>().HasData(
                new Domain.Entities.Film { Id = 1, Title = "Шоумен", Director = "Майкл Гренді", ReleaseYear = 2017, Genre = "Драма", Rating = 8.2, PhotoUrl = "https://image.tmdb.org/t/p/w342/8nFU1XNzU8fR1ElLshS8Ap0OXe1.jpg" },
                new Domain.Entities.Film { Id = 2, Title = "Аватар", Director = "Джеймс Камерон", ReleaseYear = 2009, Genre = "Науково-фантастичний", Rating = 7.8, PhotoUrl = "https://image.tmdb.org/t/p/w342/jRXYj3sqQnBA1fwWoNJ78GkbyxL.jpg" },
                new Domain.Entities.Film { Id = 3, Title = "Інтерстеллар", Director = "Крістофер Нолан", ReleaseYear = 2014, Genre = "Науково-фантастичний", Rating = 8.6, PhotoUrl = "https://image.tmdb.org/t/p/w342/xJHokMbNMnRiGqnddgS4SmHsFPM.jpg" },
                new Domain.Entities.Film { Id = 4, Title = "Один удома", Director = "Крис Коламбус", ReleaseYear = 1990, Genre = "Комедія", Rating = 7.7, PhotoUrl = "https://image.tmdb.org/t/p/w342/vqJ1hqaX9b9jDdWkMYG8O6ymscO.jpg" },
                new Domain.Entities.Film { Id = 5, Title = "Темний лицар", Director = "Крістофер Нолан", ReleaseYear = 2008, Genre = "Екшн", Rating = 9.0, PhotoUrl = "https://image.tmdb.org/t/p/w342/1hqwGsNcoWxPZV9CTZqczvQI7IZ.jpg" }
            );
        }
    }
}
