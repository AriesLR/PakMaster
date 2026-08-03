namespace PakMaster.Core.Settings
{
    public static class ConfigManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
        private static readonly SemaphoreSlim _saveLock = new(1, 1);

        public static ToolConfigModel CurrentSettings { get; set; } = new ToolConfigModel();

        public static void Initialize()
        {
            CurrentSettings = LoadConfig() ?? new ToolConfigModel();
        }

        public static ToolConfigModel? LoadConfig(string? customPath = null)
        {
            string path = customPath ?? AppConfig.ToolConfigPath;
            if (!File.Exists(path))
            {
                GLogger.Here().Warning("Tool config file not found at path: {Path}. Using default.", path);
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                var settings = JsonSerializer.Deserialize<ToolConfigModel>(json);
                return settings;
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to load tool config from: {Path}", path);
                return null;
            }
        }

        public static void SaveConfig(ToolConfigModel settings, string? customPath = null)
        {
            string path = customPath ?? AppConfig.ToolConfigPath;
            string json;
            try
            {
                json = JsonSerializer.Serialize(settings, JsonOptions);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to serialize tool config.");
                return;
            }

            _saveLock.Wait();
            try
            {
                string? directory = Path.GetDirectoryName(path);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, json);
                GLogger.Here().Debug("Tool config saved successfully to: {Path}", path);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to save tool config to disk.");
            }
            finally
            {
                _saveLock.Release();
            }
        }
    }
}