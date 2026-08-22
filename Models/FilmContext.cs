using Microsoft.EntityFrameworkCore;

namespace mvc 
{
    // контекст даних для фільмів (Film DbContext)
    // представляє собою клас, похідний від класу DbContext
    public class FilmContext : DbContext 
    {
        public DbSet<Film> Films { get; set; } // набір сутностей Film, який буде відображено в таблицю Films (ORM)
        public FilmContext(DbContextOptions<FilmContext> options) // конструктор, що приймає параметри підключення
        // options буде отриманий із Program.cs завдяки механізму впровадження залежностей (Dependency Injection)
           : base(options) // передаємо параметри базовому класу DbContext
        {
        }

        // ця функція перевіряє, чи існує база даних Postgres, і якщо ні — створює її та наповнює початковими даними
        // викликається один раз при старті застосунку з Program.cs (а не в конструкторі, бо конструктор викликається на кожен запит)
        public void EnsureDatabaseCreatedAndSeeded()
        {
            if (Database.EnsureCreated()) // повертає true, якщо БД щойно була створена (тобто раніше її не існувало)
            {
                Films.Add(new Film { Title = "Шоумен", Director = "Майкл Гренді", ReleaseYear = 2017, Genre = "Драма", Rating = 8.2, PhotoUrl = "https://image.tmdb.org/t/p/w342/8nFU1XNzU8fR1ElLshS8Ap0OXe1.jpg" }); 
                Films.Add(new Film { Title = "Аватар", Director = "Джеймс Камерон", ReleaseYear = 2009, Genre = "Науково-фантастичний", Rating = 7.8, PhotoUrl = "https://image.tmdb.org/t/p/w342/jRXYj3sqQnBA1fwWoNJ78GkbyxL.jpg" }); 
                Films.Add(new Film { Title = "Інтерстеллар", Director = "Крістофер Нолан", ReleaseYear = 2014, Genre = "Науково-фантастичний", Rating = 8.6, PhotoUrl = "https://image.tmdb.org/t/p/w342/xJHokMbNMnRiGqnddgS4SmHsFPM.jpg" }); 
                Films.Add(new Film { Title = "Один удома", Director = "Крис Коламбус", ReleaseYear = 1990, Genre = "Комедія", Rating = 7.7, PhotoUrl = "https://image.tmdb.org/t/p/w342/vqJ1hqaX9b9jDdWkMYG8O6ymscO.jpg" });
                Films.Add(new Film { Title = "Темний лицар", Director = "Крістофер Нолан", ReleaseYear = 2008, Genre = "Екшн", Rating = 9.0, PhotoUrl = "https://image.tmdb.org/t/p/w342/1hqwGsNcoWxPZV9CTZqczvQI7IZ.jpg" });
                SaveChanges(); // зберігаємо початкові дані в базу
            }
        }
    }
}
