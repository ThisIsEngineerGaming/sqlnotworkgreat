using Microsoft.EntityFrameworkCore;

namespace mvc.DAL.Repositories;

public class Repository<T>(FilmContext context) : IRepository<T> where T : class
{
    private readonly DbSet<T> _set = context.Set<T>();
    public Task<List<T>> GetAllAsync() => _set.AsNoTracking().ToListAsync();
    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
    public Task AddAsync(T entity) => _set.AddAsync(entity).AsTask();
    public void Update(T entity) => _set.Update(entity);
    public void Remove(T entity) => _set.Remove(entity);
}
