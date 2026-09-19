using Film.Domain.Interfaces;
using Film.Infrastructure.Persistence;

namespace Film.Infrastructure.Repositories
{
    /* патерн unit of work спрощує роботу з репозиторіями та гарантує,
     * що всі репозиторії використовують один і той же контекст даних. */
    public class EFUnitOfWork : IUnitOfWork
    {
        private FilmContext db;
        private IRepository<Domain.Entities.Film>? filmRepository; // поле для кешування репозиторію фільмів

        public EFUnitOfWork(FilmContext context) // конструктор, що приймає готовий контекст
        {
            db = context; // ініціалізація поля контекстом, переданим через DI
        }

        public IRepository<Domain.Entities.Film> Films // властивість для доступу до репозиторію фільмів
        {
            get
            {
                if (filmRepository == null) // перевірка, чи вже створено репозиторій
                    filmRepository = new FilmRepository(db); // створення репозиторію фільмів за потреби (lazy initialization)
                return filmRepository; // повернення єдиного екземпляра репозиторію
            }
        }

        public async Task Save() // асинхронний метод для збереження всіх змін
        {
            await db.SaveChangesAsync(); // виклик асинхронного збереження змін у контексті бази даних
        }
    }
}
