namespace PakMaster.Core.Settings
{
    public static class AppSettingsManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public static AppSettingsModel CurrentSettings { get; set; } = new AppSettingsModel();

        // Init App Settings
        public static void Initialize()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            CurrentSettings = LoadAppSettings() ?? new AppSettingsModel();

            stopwatch.Stop();

            GLogger.Here().Information("Initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
        }

        // Load AppSettings.json
        public static AppSettingsModel? LoadAppSettings()
        {
            if (!File.Exists(AppConfig.AppSettingsPath))
            {
                GLogger.Here().Warning("Configuration file not found at path: {SettingsPath}. Application will initialize with factory defaults.", AppConfig.AppSettingsPath);
                return null;
            }

            try
            {
                GLogger.Here().Debug("Attempting to read and deserialize configuration file from disk.");

                string json = File.ReadAllText(AppConfig.AppSettingsPath);
                var settings = JsonSerializer.Deserialize<AppSettingsModel>(json);

                GLogger.Here().Debug("Successfully loaded application configuration profile.");
                return settings;
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An error occurred while trying to parse the configuration file at: {SettingsPath}", AppConfig.AppSettingsPath);
                return null;
            }
        }

        private static readonly System.Threading.SemaphoreSlim _saveLock = new(1, 1);

        // Save AppSettings.json
        public static void SaveAppSettings(AppSettingsModel settings)
        {
            string json;
            try
            {
                json = JsonSerializer.Serialize(settings, JsonOptions);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to serialize application configuration settings.");
                return;
            }

            _saveLock.Wait();
            try
            {
                string? directory = Path.GetDirectoryName(AppConfig.AppSettingsPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    GLogger.Here().Information("Configuration directory does not exist. Creating directory branch: {DirectoryPath}", directory);
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(AppConfig.AppSettingsPath, json);
                GLogger.Here().Debug("Application configuration state successfully committed to disk at: {SettingsPath}", AppConfig.AppSettingsPath);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to write application configuration settings to disk.");
            }
            finally
            {
                _saveLock.Release();
            }
        }

        // Reset AppSettings.json To Factory Defaults
        public static bool ResetToFactoryDefaults()
        {
            GLogger.Here().Warning("Factory reset sequence initiated by user request.");

            try
            {
                if (File.Exists(AppConfig.AppSettingsPath))
                {
                    File.Delete(AppConfig.AppSettingsPath);
                    GLogger.Here().Information("Successfully deleted AppSettings.json file from disk.");
                }
                else
                {
                    GLogger.Here().Debug("AppSettings.json file did not exist on disk. Creating one instead.");
                }

                CurrentSettings = new AppSettingsModel();
                GLogger.Here().Debug("Application configuration restored to factory defaults.");

                SaveAppSettings(CurrentSettings);

                GLogger.Here().Information("Application factory reset sequence completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An error occurred during the application factory reset sequence.");
                return false;
            }
        }
    }
}