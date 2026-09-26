import { useI18n } from './I18nContext';

export default function LanguageSwitcher() {
    const { language, setLanguage, languages, t } = useI18n();

    return (
        <div className="lang-switcher" role="group" aria-label={t('lang.' + language)}>
            {languages.map((lng) => (
                <button
                    key={lng}
                    type="button"
                    className={`lang-btn${lng === language ? ' active' : ''}`}
                    onClick={() => setLanguage(lng)}
                    title={t(`lang.${lng}`)}
                >
                    {lng.toUpperCase()}
                </button>
            ))}
        </div>
    );
}
