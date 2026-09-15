# ASP.NET Core MVC

**Автор:** sunmeat  
**Мова проєкту:** C#  
**Фреймворк:** ASP.NET Core MVC  

## Опис проєкту

«Кінопошук» — ASP.NET Core MVC застосунок із трирівневою архітектурою.

```text
HTTP request → mvc.PL (Controller + ViewModel)
             → mvc.BLL (IFilmService, DTO, business rules)
             → mvc.DAL (IUnitOfWork, IRepository, EF Core DbContext)
             → PostgreSQL
```

`mvc.PL` містить лише HTTP/MVC-відповідальність і локальне сховище файлів. `mvc.BLL` не повертає EF-сутності: він передає DTO, перевіряє правила предметної області та працює з DAL через `IUnitOfWork`. `mvc.DAL` відповідає тільки за зберігання даних.

Контролер підтримує повний CRUD, серверну й клієнтську валідацію, одноразові повідомлення та безпечне завантаження постерів (JPEG/PNG/WebP, до 5 МБ, унікальне ім’я). Старий локальний постер видаляється лише після успішного оновлення запису.

## Основні можливості
- Класична MVC-архітектура
- Razor-шаблони (.cshtml)
- Вбудована система Dependency Injection
- Підтримка Tag Helpers
- Статичні файли у папці `wwwroot`
- Конфігурація через `appsettings.json`
- Middleware-пайплайн ASP.NET Core
- DTO та ViewModel для ізоляції шарів
- Unit of Work і generic repository

## Технології

- .NET 10
- C# 14
- Razor Views
- Entity Framework Core
