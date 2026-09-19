namespace Film.Domain.Interfaces
{
    /* Інтерфейс репозиторію оголошений у Domain (а не в Infrastructure) за принципом
     * інверсії залежностей (Dependency Inversion Principle): домен диктує контракт
     * доступу до даних, а Infrastructure (EF Core) лише реалізує його. */
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> Get(int id);
        Task<T?> Get(string name);
        Task Create(T item);
        void Update(T item);
        Task Delete(int id);
    }
}
