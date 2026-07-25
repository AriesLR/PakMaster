namespace PakMaster.Infrastructure.Styling
{
    public static class ThemeBuilder
    {
        public static List<string> AvailableBaseThemes { get; private set; } = [];
        public static List<Theme> AvailableAccents { get; private set; } = [];

        public static void Initialize(AppSettingsModel settings)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                AvailableBaseThemes = [.. ThemeManager.Current.Themes
                    .GroupBy(x => x.BaseColorScheme)
                    .Select(g => g.Key)
                    .OrderBy(s => s)];

                AvailableAccents = [.. ThemeManager.Current.Themes
                    .GroupBy(x => x.ColorScheme)
                    .Select(g => g.First())
                    .OrderBy(t => t.ColorScheme)];

                GLogger.Here().Debug("Discovered {BaseThemeCount} base themes and {AccentCount} color accents from ControlzEx.", AvailableBaseThemes.Count, AvailableAccents.Count);

                ApplyTheme(settings.BaseTheme, settings.AccentColor);

                stopwatch.Stop();

                GLogger.Here().Information("Initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to initialize application themes. Core theme engine failed.");
            }
        }

        public static void SetBaseTheme(string baseTheme)
        {
            if (string.IsNullOrWhiteSpace(baseTheme))
            {
                GLogger.Here().Warning("SetBaseTheme rejected: Provided base theme string was null or empty.");
                return;
            }

            GLogger.Here().Debug("Changing base theme to: {BaseTheme}", baseTheme);
            AppSettingsManager.CurrentSettings.BaseTheme = baseTheme;
            ApplyTheme(baseTheme, AppSettingsManager.CurrentSettings.AccentColor);
        }

        public static void SetAccentColor(string accentColor)
        {
            if (string.IsNullOrWhiteSpace(accentColor))
            {
                GLogger.Here().Warning("SetAccentColor rejected: Provided accent color string was null or empty.");
                return;
            }

            GLogger.Here().Debug("Changing color accent to: {AccentColor}", accentColor);
            AppSettingsManager.CurrentSettings.AccentColor = accentColor;
            ApplyTheme(AppSettingsManager.CurrentSettings.BaseTheme, accentColor);
        }

        private static void ApplyTheme(string baseTheme, string accentColor)
        {
            try
            {
                ThemeManager.Current.ChangeTheme(Application.Current, baseTheme, accentColor);

                GLogger.Here().Information("Theme service successfully rendered {BaseTheme}.{AccentColor}", baseTheme, accentColor);
                AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to apply theme combination. Base: {BaseTheme}, Accent: {AccentColor}", baseTheme, accentColor);
            }
        }
    }
}