using mvc.DAL.Entities;
using mvc.DAL.Repositories;

namespace mvc.DAL;

public interface IUnitOfWork
{
    IRepository<Film> Films { get; }
    Task<int> SaveChangesAsync();
}
