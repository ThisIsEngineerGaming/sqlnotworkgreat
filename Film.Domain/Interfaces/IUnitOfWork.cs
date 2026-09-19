using Film.Domain.Entities;

namespace Film.Domain.Interfaces
{
    // координує роботу репозиторіїв та збереження змін у межах однієї логічної одиниці роботи
    public interface IUnitOfWork
    {
        IRepository<Entities.Film> Films { get; } // доступ до репозиторію фільмів
        Task Save(); // асинхронне збереження всіх змін у базі даних
    }
}
