# Кінопошук — ASP.NET Core Web API + React

**Автор:** sunmeat
**Мова проєкту:** C# / JavaScript (React)
**Фреймворк:** ASP.NET Core Web API (Clean Architecture) + Vite/React

## Опис проєкту

«Кінопошук» — застосунок для каталогу фільмів, розділений на два незалежні застосунки:
бекенд (ASP.NET Core Web API, Clean Architecture, 5 проєктів) та фронтенд (React + TanStack Table,
Vite dev server). Це переробка попередньої версії на класичному ASP.NET Core MVC
(яка збережена без змін у `_legacy_mvc/` для довідки).

```text
Browser (film.client, React + TanStack Table)
        │  fetch('/api/films') — проксується Vite на бекенд у режимі розробки
        ▼
Film.WebAPI (Controllers, композиційний корінь)
        │
        ▼
Film.Application (IEntityService<FilmDTO>, FilmService, AutoMapper-профіль, бізнес-правила)
        │
        ▼
Film.Domain (сутність Film, IRepository, IUnitOfWork — жодних залежностей)
        ▲
        │  реалізує контракти Domain
Film.Infrastructure (EF Core, FilmContext, FilmRepository, EFUnitOfWork, PostgreSQL)

Film.Common — наскрізні речі (ValidationException, BusinessRuleException),
              видимі і Application, і Presentation.
```

Напрямок залежностей суворо всередину — до Domain, за принципами Clean Architecture /
Dependency Inversion Principle: Domain нічого не знає про EF Core чи ASP.NET Core,
а Infrastructure та Application лише реалізують чи споживають його контракти.

## Структура репозиторію

| Проєкт               | Призначення                                                         |
|-----------------------|----------------------------------------------------------------------|
| `Film.Domain`         | Сутність `Film`, `IRepository<T>`, `IUnitOfWork` — центр архітектури |
| `Film.Common`          | `ValidationException`, `BusinessRuleException`                      |
| `Film.Application`    | `FilmDTO`, `IEntityService<TDto>`, `FilmService`, AutoMapper-профіль |
| `Film.Infrastructure` | `FilmContext` (EF Core + PostgreSQL), репозиторій, Unit of Work      |
| `Film.WebAPI`         | `FilmsController` (`api/films`), композиційний корінь, `Program.cs`  |
| `film.client`         | React SPA (Vite), TanStack Table з сортуванням, форма CRUD           |
| `_legacy_mvc`         | Оригінальна версія на класичному ASP.NET Core MVC (`mvc.DAL/BLL/PL`) |

## Основні можливості

- Чиста архітектура (Domain / Application / Infrastructure / Presentation)
- REST API: `GET/POST/PUT/DELETE api/films`
- React SPA з таблицею на `@tanstack/react-table` (клікабельне сортування колонок)
- CRUD-форма з валідацією на клієнті та сервері (DataAnnotations)
- Бізнес-правила в шарі Application (унікальність назви фільму, коректний рік/рейтинг)
- AutoMapper для проєкції сутність → DTO
- PostgreSQL через Npgsql + EF Core, посів початкових даних через `HasData`
- SpaProxy для безшовної розробки: бекенд автоматично піднімає `npm run dev`

## Запуск у розробці

1. Відкрити `Film.slnx` у Visual Studio (або `dotnet run --project Film.WebAPI`).
2. Переконайтесь, що PostgreSQL піднятий і рядок підключення в
   `Film.WebAPI/appsettings.json` відповідає вашому середовищу.
3. Запустити `Film.WebAPI` — SpaProxy автоматично виконає `npm install`/`npm run dev`
   у `film.client` і відкриє React-застосунок на `https://localhost:3001`
   (проксує `/api/*` на `https://localhost:5050`).

## Технології

- .NET 10, C# 14
- ASP.NET Core Web API, AutoMapper
- Entity Framework Core + Npgsql (PostgreSQL)
- React 19, Vite, `@tanstack/react-table`
