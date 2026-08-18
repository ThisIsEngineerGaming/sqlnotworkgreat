using Microsoft.EntityFrameworkCore;

namespace mvc 
{
    // щоб підключитися до бази даних через Entity Framework, необхідний контекст даних
    // контекст даних представляє собою клас, похідний від класу DbContext
    public class StudentContext : DbContext 
    {
        public DbSet<Student> Students { get; set; } // набір сутностей Student, який буде відображено в таблицю Students (ORM)
        public StudentContext(DbContextOptions<StudentContext> options) // конструктор, що приймає параметри підключення
        // options буде отриманий із Program.cs завдяки механізму впровадження залежностей (Dependency Injection)
           : base(options) // передаємо параметри базовому класу DbContext
        {
            if (Database.EnsureCreated()) // якщо база даних ще не створена — створюємо її (одноразово)
            {
                Students?.Add(new Student { Name = "Руслан", Surname = "Мельник", Age = 20, GPA = 10.5 }); // GPA - середній бал, grade point average
                Students?.Add(new Student { Name = "Максим", Surname = "Шевченко", Age = 23, GPA = 11.5 }); 
                Students?.Add(new Student { Name = "Денис", Surname = "Коваленко", Age = 25, GPA = 12 }); 
                Students?.Add(new Student { Name = "Марія", Surname = "Бондаренко", Age = 22, GPA = 9.5 });
                Students?.Add(new Student { Name = "Софія", Surname = "Ткаченко", Age = 24, GPA = 11.2 });
                SaveChanges(); // зберігаємо початкові дані в базу
            }
        }
    }
}