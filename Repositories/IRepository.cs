namespace mvc.Repositories
{
    // Узагальнений (generic) інтерфейс репозиторію.
    // Описує контракт доступу до даних для будь-якої сутності T (у нашому
    // випадку — Film), не прив'язуючись до конкретного DbContext чи БД.
    //
    // Саме через цей інтерфейс сервісний шар (FilmService) тепер звертається
    // до даних — а не напряму до FilmContext. Це і є впровадження принципу
    // інверсії залежностей (Dependency Inversion Principle, DIP): сервіс
    // залежить від абстракції (IRepository<T>), а не від конкретної
    // реалізації доступу до БД.
    public interface IRepository<T> where T : class
    {
        // отримати список усіх сутностей
        Task<List<T>> GetAllAsync();

        // отримати одну сутність за id (або null, якщо не знайдено)
        Task<T?> GetByIdAsync(int id);

        // додати нову сутність у контекст (без збереження в БД —
        // для цього окремо викликається SaveChangesAsync)
        Task AddAsync(T entity);

        // позначити сутність як змінену. синхронний, бо EF Core Update —
        // це суто in-memory операція над ChangeTracker'ом, без звернення до БД
        void Update(T entity);

        // видалити сутність за id (якщо існує)
        Task DeleteAsync(int id);

        // перевірити, чи існує сутність із заданим id
        Task<bool> ExistsAsync(int id);

        // зберегти всі накопичені зміни в базі даних
        Task SaveChangesAsync();
    }
}
