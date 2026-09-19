using Microsoft.EntityFrameworkCore;
using Film.Domain.Interfaces;
using Film.Infrastructure.Persistence;

namespace Film.Infrastructure.Repositories
{
    public class FilmRepository : IRepository<Domain.Entities.Film>
    {
        private FilmContext db;

        public FilmRepository(FilmContext context) // конструктор, що приймає контекст через DI
        {
            db = context;
        }

        public async Task<IEnumerable<Domain.Entities.Film>> GetAll() // отримання всіх фільмів
        {
            return await db.Films.AsNoTracking().ToListAsync();
        }

        public async Task<Domain.Entities.Film?> Get(int id) // асинхронний метод для отримання фільму за ідентифікатором
        {
            return await db.Films.FindAsync(id);
        }

        public async Task<Domain.Entities.Film?> Get(string name) // асинхронний метод для отримання фільму за назвою
        {
            return await db.Films.FirstOrDefaultAsync(f => f.Title == name);
        }

        public async Task Create(Domain.Entities.Film film) // асинхронний метод для створення нового фільму
        {
            await db.Films.AddAsync(film); // додаємо сутність до dbset асинхронно (зміни зберігаються пізніше через savechanges)
        }

        public void Update(Domain.Entities.Film film) // синхронний метод для оновлення фільму
        {
            db.Entry(film).State = EntityState.Modified; // явно позначаємо сутність як змінену для відстеження EF
        }

        public async Task Delete(int id) // асинхронний метод для видалення фільму за ідентифікатором
        {
            var film = await db.Films.FindAsync(id); // ефективний пошук за первинним ключем асинхронно
            if (film != null) // перевірка на існування
                db.Films.Remove(film); // видаляємо сутність з dbset (зміни застосовуються при savechanges)
        }
    }
}
