using mvc.DAL.Entities;
using mvc.DAL.Repositories;

namespace mvc.DAL;

public sealed class UnitOfWork(FilmContext context, IRepository<Film> films) : IUnitOfWork
{
    public IRepository<Film> Films { get; } = films;
    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
