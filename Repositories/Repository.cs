using Microsoft.EntityFrameworkCore;
using mvc;

namespace mvc.Repositories
{
    // Конкретна (generic) реалізація IRepository<T>.
    // Це єдине місце в усьому додатку, де відбувається реальна робота
    // з FilmContext (DbContext) — жоден контролер чи сервіс більше
    // напряму до контексту не звертається.
    public class Repository<T> : IRepository<T> where T : class
    {
        // поле зберігає контекст БД, який інжектується через DI (Dependency Injection).
        // readonly — щоб контекст не можна було випадково підмінити після створення репозиторію.
        private readonly FilmContext _context;

        // DbSet<T> — конкретна таблиця (набір сутностей) всередині контексту,
        // з якою працює цей екземпляр репозиторію (наприклад, Films).
        private readonly DbSet<T> _dbSet;

        public Repository(FilmContext context) // Dependency Injection: контекст передається через конструктор
        {
            _context = context;
            _dbSet = _context.Set<T>(); // Set<T>() дозволяє отримати DbSet потрібної сутності узагальнено
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity); // позначає сутність як Modified у ChangeTracker'і, без звернення до БД
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id); // шукає сутність для видалення
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
