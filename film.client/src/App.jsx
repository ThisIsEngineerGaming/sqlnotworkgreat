import { useState, useEffect, useCallback, useMemo } from 'react';
import {
    useTable,
    tableFeatures,
    rowSortingFeature,
    createSortedRowModel,
    createColumnHelper,
} from '@tanstack/react-table';

import './App.css';

const API_BASE_URL = '/api';

const columnHelper = createColumnHelper();

const sortableTableFeatures = tableFeatures({
    rowSortingFeature,
    sortedRowModel: createSortedRowModel(),
});

const emptyForm = { id: 0, title: '', director: '', releaseYear: new Date().getFullYear(), genre: '', rating: '', photoUrl: '' };

function SortableTh({ table, column, children }) {
    const col = table.getColumn(column);
    const direction = col?.getIsSorted();
    return (
        <th className="sortable" onClick={col?.getToggleSortingHandler()}>
            {children}
            {direction && <span className="sort-arrow">{direction === 'asc' ? '▲' : '▼'}</span>}
        </th>
    );
}

function RatingBadge({ value }) {
    if (value === undefined || value === null || value === '') return <span className="badge badge-muted">—</span>;
    const n = Number(value);
    const tone = n >= 8 ? 'badge-gold' : n >= 6 ? 'badge-accent' : 'badge-muted';
    return <span className={`badge ${tone}`}>★ {n.toFixed(1)}</span>;
}

export default function App() {
    const [films, setFilms] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [form, setForm] = useState(emptyForm);
    const [isEditing, setIsEditing] = useState(false);
    const [sorting, setSorting] = useState([]);
    const [confirmDialog, setConfirmDialog] = useState(null);

    const columns = useMemo(
        () => [
            columnHelper.accessor('id', { header: 'ID' }),
            columnHelper.accessor('photoUrl', { header: 'Постер', enableSorting: false }),
            columnHelper.accessor('title', { header: 'Назва' }),
            columnHelper.accessor('director', { header: 'Режисер' }),
            columnHelper.accessor('releaseYear', { header: 'Рік' }),
            columnHelper.accessor('genre', { header: 'Жанр' }),
            columnHelper.accessor('rating', { header: 'Рейтинг' }),
        ],
        []
    );

    const table = useTable({
        features: sortableTableFeatures,
        data: films,
        columns,
        state: { sorting },
        onSortingChange: setSorting,
    });

    const sortedFilms = table.getRowModel().rows.map((row) => row.original);

    const resetForm = useCallback(() => {
        setForm({ ...emptyForm, releaseYear: new Date().getFullYear() });
        setIsEditing(false);
    }, []);

    const fetchFilms = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await fetch(`${API_BASE_URL}/films`);
            if (!response.ok) throw new Error(`Помилка завантаження: ${response.statusText}`);
            const data = await response.json();
            setFilms(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchFilms();
    }, [fetchFilms]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        const payload = {
            id: form.id,
            title: form.title,
            director: form.director,
            releaseYear: Number(form.releaseYear),
            genre: form.genre,
            rating: Number(form.rating),
            photoUrl: form.photoUrl || null,
        };
        const url = isEditing ? `${API_BASE_URL}/films/${payload.id}` : `${API_BASE_URL}/films`;
        const method = isEditing ? 'PUT' : 'POST';

        try {
            const response = await fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload),
            });
            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || 'Не вдалося зберегти фільм');
            }
            resetForm();
            fetchFilms();
        } catch (err) {
            setError(err.message);
        }
    };

    const handleEdit = (film) => {
        setIsEditing(true);
        setForm({
            id: film.id,
            title: film.title || '',
            director: film.director || '',
            releaseYear: film.releaseYear ?? new Date().getFullYear(),
            genre: film.genre || '',
            rating: film.rating ?? '',
            photoUrl: film.photoUrl || '',
        });
    };

    const requestDelete = (id, title) => setConfirmDialog({ id, title });
    const cancelDelete = () => setConfirmDialog(null);

    const confirmDelete = async () => {
        if (!confirmDialog) return;
        const id = confirmDialog.id;
        setConfirmDialog(null);
        try {
            const response = await fetch(`${API_BASE_URL}/films/${id}`, { method: 'DELETE' });
            if (!response.ok) throw new Error('Помилка при видаленні');
            fetchFilms();
        } catch (err) {
            setError(err.message);
        }
    };

    return (
        <div className="app">
            <header className="header">
                <div className="header-inner">
                    <div className="brand">
                        <span className="brand-icon">🎬</span>
                        <div className="brand-text">
                            <span className="brand-title">Кіно каталог</span>
                            <span className="brand-sub">Менеджер фільмів</span>
                        </div>
                    </div>
                </div>
            </header>

            <main className="main">
                {error && <div className="alert">{error}</div>}

                <div className="layout">
                    <section className="panel form-panel">
                        <div className="panel-head">
                            <h2>{isEditing ? 'Редагувати' : 'Додати'} фільм</h2>
                        </div>
                        <form onSubmit={handleSubmit} className="form">
                            <div className="field">
                                <label>Назва фільму</label>
                                <input
                                    type="text"
                                    required
                                    value={form.title}
                                    onChange={(e) => setForm({ ...form, title: e.target.value })}
                                    placeholder="наприклад, Темний лицар"
                                />
                            </div>
                            <div className="field">
                                <label>Режисер</label>
                                <input
                                    type="text"
                                    required
                                    value={form.director}
                                    onChange={(e) => setForm({ ...form, director: e.target.value })}
                                    placeholder="наприклад, Крістофер Нолан"
                                />
                            </div>
                            <div className="field">
                                <label>Рік випуску</label>
                                <input
                                    type="number"
                                    required
                                    min="1888"
                                    max="2100"
                                    value={form.releaseYear}
                                    onChange={(e) => setForm({ ...form, releaseYear: e.target.value })}
                                />
                            </div>
                            <div className="field">
                                <label>Жанр</label>
                                <input
                                    type="text"
                                    required
                                    value={form.genre}
                                    onChange={(e) => setForm({ ...form, genre: e.target.value })}
                                    placeholder="наприклад, Екшн"
                                />
                            </div>
                            <div className="field">
                                <label>Рейтинг (0-10)</label>
                                <input
                                    type="number"
                                    required
                                    step="0.1"
                                    min="0"
                                    max="10"
                                    value={form.rating}
                                    onChange={(e) => setForm({ ...form, rating: e.target.value })}
                                    placeholder="8.5"
                                />
                            </div>
                            <div className="field">
                                <label>Посилання на постер</label>
                                <input
                                    type="url"
                                    value={form.photoUrl}
                                    onChange={(e) => setForm({ ...form, photoUrl: e.target.value })}
                                    placeholder="https://..."
                                />
                            </div>

                            <div className="form-actions">
                                <button type="submit" className="btn btn-primary">
                                    {isEditing ? 'Зберегти' : 'Створити'}
                                </button>
                                {isEditing && (
                                    <button type="button" className="btn btn-ghost" onClick={resetForm}>
                                        Скасувати
                                    </button>
                                )}
                            </div>
                        </form>
                    </section>

                    <section className="panel list-panel">
                        <div className="panel-head">
                            <h2>Список фільмів</h2>
                            <span className="count">{sortedFilms.length} записів</span>
                        </div>

                        {loading ? (
                            <div className="loader">
                                <div className="spinner"></div>
                                <span>Завантаження...</span>
                            </div>
                        ) : (
                            <div className="table-wrap">
                                <table>
                                    <thead>
                                        <tr>
                                            <SortableTh table={table} column="id">ID</SortableTh>
                                            <th>Постер</th>
                                            <SortableTh table={table} column="title">Назва</SortableTh>
                                            <SortableTh table={table} column="director">Режисер</SortableTh>
                                            <SortableTh table={table} column="releaseYear">Рік</SortableTh>
                                            <SortableTh table={table} column="genre">Жанр</SortableTh>
                                            <SortableTh table={table} column="rating">Рейтинг</SortableTh>
                                            <th>Дії</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {sortedFilms.length === 0 ? (
                                            <tr>
                                                <td colSpan={8} className="empty">Дані відсутні</td>
                                            </tr>
                                        ) : (
                                            sortedFilms.map((film) => (
                                                <tr key={film.id}>
                                                    <td className="id">#{film.id}</td>
                                                    <td className="poster-cell">
                                                        {film.photoUrl ? (
                                                            <img className="poster-thumb" src={film.photoUrl} alt={film.title} />
                                                        ) : (
                                                            <span className="poster-placeholder">—</span>
                                                        )}
                                                    </td>
                                                    <td className="name">{film.title}</td>
                                                    <td>{film.director}</td>
                                                    <td>{film.releaseYear}</td>
                                                    <td>
                                                        <span className="badge badge-accent">{film.genre}</span>
                                                    </td>
                                                    <td><RatingBadge value={film.rating} /></td>
                                                    <td className="actions">
                                                        <button className="icon-btn edit" onClick={() => handleEdit(film)} title="Редагувати">
                                                            ✎
                                                        </button>
                                                        <button className="icon-btn delete" onClick={() => requestDelete(film.id, film.title)} title="Видалити">
                                                            ✕
                                                        </button>
                                                    </td>
                                                </tr>
                                            ))
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </section>
                </div>
            </main>

            <footer className="footer">
                <div className="footer-inner">
                    <div className="footer-brand">
                        <span className="brand-icon">🎬</span>
                        <span>Кіно каталог</span>
                    </div>
                    <p>Приклад на Clean Architecture: ASP.NET Core Web API + React (розділено з мono-MVC)</p>
                    <p className="footer-copy">© {new Date().toLocaleString('uk-UA')}</p>
                </div>
            </footer>

            {confirmDialog && (
                <div className="modal-overlay" onClick={cancelDelete}>
                    <div className="modal" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-icon">⚠</div>
                        <h3 className="modal-title">Видалити запис?</h3>
                        <p className="modal-text">
                            Фільм «{confirmDialog.title}» буде видалено назавжди. Цю дію неможливо скасувати.
                        </p>
                        <div className="modal-actions">
                            <button className="btn btn-ghost" onClick={cancelDelete}>
                                Скасувати
                            </button>
                            <button className="btn btn-danger" onClick={confirmDelete}>
                                Видалити
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
