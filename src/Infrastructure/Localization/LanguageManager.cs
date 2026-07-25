namespace PakMaster.Infrastructure.Localization
{
    public static class LanguageManager
    {
        public static event EventHandler? LanguageChanged;

        // Supported Languages
        public static readonly List<LanguageModel> SupportedLanguages =
        [
            new LanguageModel { DisplayName = "English", CultureCode = "en" },
            new LanguageModel { DisplayName = "Español", CultureCode = "es" },
            new LanguageModel { DisplayName = "Português", CultureCode = "pt" },
            new LanguageModel { DisplayName = "Tiếng Việt", CultureCode = "vi" },
            new LanguageModel { DisplayName = "Русский", CultureCode = "ru" },
            new LanguageModel { DisplayName = "Українська", CultureCode = "uk" },
            new LanguageModel { DisplayName = "العربية", CultureCode = "ar" },
            new LanguageModel { DisplayName = "বাংলা", CultureCode = "bn" },
            new LanguageModel { DisplayName = "हिन्दी", CultureCode = "hi" },
            new LanguageModel { DisplayName = "日本語", CultureCode = "ja" },
            new LanguageModel { DisplayName = "简体中文", CultureCode = "zh-Hans" },
            new LanguageModel { DisplayName = "繁體中文", CultureCode = "zh-Hant" }
        ];

        public static bool IsRightToLeft
        {
            get
            {
                string langCode = AppSettingsManager.CurrentSettings?.Language ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                if (string.IsNullOrWhiteSpace(langCode))
                    return false;

                string cleanCode = langCode.Split('-')[0].ToLowerInvariant();

                return cleanCode is "ar" or "he" or "fa" or "ur" or "ps" or "dv" or "syr";
            }
        }

        public static FlowDirection CurrentFlowDirection => IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        public static void Initialize(AppSettingsModel settings)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var cultureCode = settings?.Language;

            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                cultureCode = "en";
            }

            ApplyLanguage(cultureCode);
            LanguageChanged?.Invoke(null, EventArgs.Empty);

            stopwatch.Stop();

            GLogger.Here().Information("Initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
        }

        public static void SetLanguage(string cultureCode)
        {
            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                GLogger.Here().Warning("SetLanguage rejected: Provided culture string was null or empty.");
                return;
            }

            GLogger.Here().Debug("Changing UI language to: {CultureCode}", cultureCode);

            AppSettingsManager.CurrentSettings.Language = cultureCode;
            AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);

            ApplyLanguage(cultureCode);
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void ApplyLanguage(string cultureCode)
        {
            try
            {
                var culture = CultureInfo.CreateSpecificCulture(cultureCode);

                LocalizeDictionary.Instance.Culture = culture;

                Lang.Culture = culture;

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
            catch (CultureNotFoundException ex)
            {
                GLogger.Here().Error(ex, "Failed to apply invalid culture code: {CultureCode}. Falling back to default.", cultureCode);

                var fallbackInfo = new CultureInfo("en");
                LocalizeDictionary.Instance.Culture = fallbackInfo;
                Lang.Culture = fallbackInfo;
                CultureInfo.CurrentCulture = fallbackInfo;
                CultureInfo.CurrentUICulture = fallbackInfo;
            }
        }
    }
}