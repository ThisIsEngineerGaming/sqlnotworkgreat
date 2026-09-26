import { useState, useEffect, useCallback, useMemo } from 'react';
import {
    useTable,
    tableFeatures,
    rowSortingFeature,
    createSortedRowModel,
    createColumnHelper,
} from '@tanstack/react-table';

import './App.css';
import { useI18n } from './i18n/I18nContext';
import LanguageSwitcher from './i18n/LanguageSwitcher';

// На Render задається змінна VITE_API_URL (повна адреса API); локально працює проксі Vite через '/api'
const API_BASE_URL = import.meta.env.VITE_API_URL ?? '/api';

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
    const { t, localeTag } = useI18n();
    const [films, setFilms] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [form, setForm] = useState(emptyForm);
    const [isEditing, setIsEditing] = useState(false);
    const [sorting, setSorting] = useState([]);
    const [confirmDialog, setConfirmDialog] = useState(null);

    const columns = useMemo(
        () => [
            columnHelper.accessor('id', { header: t('table.colId') }),
            columnHelper.accessor('photoUrl', { header: t('table.colPoster'), enableSorting: false }),
            columnHelper.accessor('title', { header: t('table.colTitle') }),
            columnHelper.accessor('director', { header: t('table.colDirector') }),
            columnHelper.accessor('releaseYear', { header: t('table.colYear') }),
            columnHelper.accessor('genre', { header: t('table.colGenre') }),
            columnHelper.accessor('rating', { header: t('table.colRating') }),
        ],
        [t]
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
            if (!response.ok) throw new Error(t('errors.load', { status: response.statusText }));
            const data = await response.json();
            setFilms(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, [t]);

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
                throw new Error(message || t('errors.save'));
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
            if (!response.ok) throw new Error(t('errors.delete'));
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
                            <span className="brand-title">{t('app.title')}</span>
                            <span className="brand-sub">{t('app.subtitle')}</span>
                        </div>
                    </div>
                    <LanguageSwitcher />
                </div>
            </header>

            <main className="main">
                {error && <div className="alert">{error}</div>}

                <div className="layout">
                    <section className="panel form-panel">
                        <div className="panel-head">
                            <h2>{isEditing ? t('form.editTitle') : t('form.addTitle')}</h2>
                        </div>
                        <form onSubmit={handleSubmit} className="form">
                            <div className="field">
                                <label>{t('form.titleLabel')}</label>
                                <input
                                    type="text"
                                    required
                                    value={form.title}
                                    onChange={(e) => setForm({ ...form, title: e.target.value })}
                                    placeholder={t('form.titlePlaceholder')}
                                />
                            </div>
                            <div className="field">
                                <label>{t('form.directorLabel')}</label>
                                <input
                                    type="text"
                                    required
                                    value={form.director}
                                    onChange={(e) => setForm({ ...form, director: e.target.value })}
                                    placeholder={t('form.directorPlaceholder')}
                                />
                            </div>
                            <div className="field">
                                <label>{t('form.yearLabel')}</label>
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
                                <label>{t('form.genreLabel')}</label>
                                <input
                                    type="text"
                                    required
                                    value={form.genre}
                                    onChange={(e) => setForm({ ...form, genre: e.target.value })}
                                    placeholder={t('form.genrePlaceholder')}
                                />
                            </div>
                            <div className="field">
                                <label>{t('form.ratingLabel')}</label>
                                <input
                                    type="number"
                                    required
                                    step="0.1"
                                    min="0"
                                    max="10"
                                    value={form.rating}
                                    onChange={(e) => setForm({ ...form, rating: e.target.value })}
                                    placeholder={t('form.ratingPlaceholder')}
                                />
                            </div>
                            <div className="field">
                                <label>{t('form.posterLabel')}</label>
                                <input
                                    type="url"
                                    value={form.photoUrl}
                                    onChange={(e) => setForm({ ...form, photoUrl: e.target.value })}
                                    placeholder={t('form.posterPlaceholder')}
                                />
                            </div>

                            <div className="form-actions">
                                <button type="submit" className="btn btn-primary">
                                    {isEditing ? t('form.save') : t('form.create')}
                                </button>
                                {isEditing && (
                                    <button type="button" className="btn btn-ghost" onClick={resetForm}>
                                        {t('form.cancel')}
                                    </button>
                                )}
                            </div>
                        </form>
                    </section>

                    <section className="panel list-panel">
                        <div className="panel-head">
                            <h2>{t('table.listTitle')}</h2>
                            <span className="count">{sortedFilms.length} {t('table.records')}</span>
                        </div>

                        {loading ? (
                            <div className="loader">
                                <div className="spinner"></div>
                                <span>{t('table.loading')}</span>
                            </div>
                        ) : (
                            <div className="table-wrap">
                                <table>
                                    <thead>
                                        <tr>
                                            <SortableTh table={table} column="id">{t('table.colId')}</SortableTh>
                                            <th>{t('table.colPoster')}</th>
                                            <SortableTh table={table} column="title">{t('table.colTitle')}</SortableTh>
                                            <SortableTh table={table} column="director">{t('table.colDirector')}</SortableTh>
                                            <SortableTh table={table} column="releaseYear">{t('table.colYear')}</SortableTh>
                                            <SortableTh table={table} column="genre">{t('table.colGenre')}</SortableTh>
                                            <SortableTh table={table} column="rating">{t('table.colRating')}</SortableTh>
                                            <th>{t('table.colActions')}</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {sortedFilms.length === 0 ? (
                                            <tr>
                                                <td colSpan={8} className="empty">{t('table.noData')}</td>
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
                                                        <button className="icon-btn edit" onClick={() => handleEdit(film)} title={t('table.edit')}>
                                                            ✎
                                                        </button>
                                                        <button className="icon-btn delete" onClick={() => requestDelete(film.id, film.title)} title={t('table.delete')}>
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
                        <span>{t('app.title')}</span>
                    </div>
                    <p>{t('footer.description')}</p>
                    <p className="footer-copy">© {new Date().toLocaleString(localeTag)}</p>
                </div>
            </footer>

            {confirmDialog && (
                <div className="modal-overlay" onClick={cancelDelete}>
                    <div className="modal" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-icon">⚠</div>
                        <h3 className="modal-title">{t('modal.title')}</h3>
                        <p className="modal-text">
                            {t('modal.text', { film: confirmDialog.title })}
                        </p>
                        <div className="modal-actions">
                            <button className="btn btn-ghost" onClick={cancelDelete}>
                                {t('modal.cancel')}
                            </button>
                            <button className="btn btn-danger" onClick={confirmDelete}>
                                {t('modal.confirm')}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
