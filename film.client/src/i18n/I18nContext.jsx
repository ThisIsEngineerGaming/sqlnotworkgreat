import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import uk from './locales/uk.json';
import en from './locales/en.json';
import pl from './locales/pl.json';

// Словники перекладів, підвантажені як звичайні JSON-модулі (підтримується Vite з коробки)
const RESOURCES = { uk, en, pl };

// Мапа для коректного локале-залежного форматування дат/чисел (toLocaleString тощо)
const LOCALE_TAGS = { uk: 'uk-UA', en: 'en-US', pl: 'pl-PL' };

export const SUPPORTED_LANGUAGES = ['uk', 'en', 'pl'];
const DEFAULT_LANGUAGE = 'uk';
const STORAGE_KEY = 'app.language';

const I18nContext = createContext(null);

function getInitialLanguage() {
    try {
        const saved = window.localStorage.getItem(STORAGE_KEY);
        if (saved && SUPPORTED_LANGUAGES.includes(saved)) return saved;
    } catch {
        // localStorage може бути недоступний (приватний режим тощо) - тихо ігноруємо
    }
    const browserLang = navigator.language?.slice(0, 2);
    return SUPPORTED_LANGUAGES.includes(browserLang) ? browserLang : DEFAULT_LANGUAGE;
}

// Дістає значення за ключем виду "form.titleLabel" з вкладеного об'єкта перекладів
function resolve(dict, key) {
    return key.split('.').reduce((acc, part) => (acc && acc[part] !== undefined ? acc[part] : undefined), dict);
}

// Підставляє параметри виду {{film}} у рядок перекладу
function interpolate(template, params) {
    if (!params || typeof template !== 'string') return template;
    return Object.keys(params).reduce(
        (str, param) => str.replaceAll(`{{${param}}}`, params[param]),
        template
    );
}

export function I18nProvider({ children }) {
    const [language, setLanguageState] = useState(getInitialLanguage);

    const setLanguage = useCallback((lang) => {
        if (!SUPPORTED_LANGUAGES.includes(lang)) return;
        setLanguageState(lang);
        try {
            window.localStorage.setItem(STORAGE_KEY, lang);
        } catch {
            // ігноруємо помилки запису в localStorage
        }
    }, []);

    // Синхронізуємо <html lang="..."> з обраною мовою
    useEffect(() => {
        document.documentElement.lang = language;
    }, [language]);

    const t = useCallback(
        (key, params) => {
            const value =
                resolve(RESOURCES[language], key) ?? resolve(RESOURCES[DEFAULT_LANGUAGE], key) ?? key;
            return interpolate(value, params);
        },
        [language]
    );

    const localeTag = LOCALE_TAGS[language] ?? LOCALE_TAGS[DEFAULT_LANGUAGE];

    const value = useMemo(
        () => ({ language, setLanguage, t, localeTag, languages: SUPPORTED_LANGUAGES }),
        [language, setLanguage, t, localeTag]
    );

    return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>;
}

export function useI18n() {
    const ctx = useContext(I18nContext);
    if (!ctx) throw new Error('useI18n must be used within an <I18nProvider>');
    return ctx;
}
