namespace PakMaster.Core.Settings
{
    public static class ConfigManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
        private static readonly SemaphoreSlim _saveLock = new(1, 1);

        public static ToolConfigModel CurrentSettings { get; set; } = new ToolConfigModel();
        public static event Action? ProfileChanged;

        public static void Initialize()
        {
            string activeProfileName = AppSettingsManager.CurrentSettings?.ActiveProfileName ?? "tools-config.json";
            string profilePath = Path.Combine(AppConfig.PakMasterConfigsFolder, activeProfileName);
            CurrentSettings = LoadConfig(profilePath) ?? new ToolConfigModel();
        }

        public static List<string> GetAvailableProfiles()
        {
            var profiles = new List<string>();
            try
            {
                if (!Directory.Exists(AppConfig.PakMasterConfigsFolder))
                {
                    Directory.CreateDirectory(AppConfig.PakMasterConfigsFolder);
                }

                string[] jsonFiles = Directory.GetFiles(AppConfig.PakMasterConfigsFolder, "*.json");
                
                foreach (string file in jsonFiles)
                {
                    string fileName = Path.GetFileName(file); 
                    
                    profiles.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to get available profiles.");
            }
            
            return profiles;
        }

        public static ToolConfigModel CreateProfile(string name)
        {
            if (!name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                name += ".json";
            }
            
            string path = Path.Combine(AppConfig.PakMasterConfigsFolder, name);
            var newProfile = new ToolConfigModel();
            SaveConfig(newProfile, path);
            return newProfile;
        }

        public static bool DeleteProfile(string name)
        {
            try
            {
                if (!name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    name += ".json";
                }
                
                string path = Path.Combine(AppConfig.PakMasterConfigsFolder, name);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to delete profile: {ProfileName}", name);
            }
            return false;
        }

        public static void ResetProfile()
        {
            CurrentSettings = new ToolConfigModel();
            
            string activeProfileName = AppSettingsManager.CurrentSettings?.ActiveProfileName ?? "tools-config.json";
            string path = Path.Combine(AppConfig.PakMasterConfigsFolder, activeProfileName);
            SaveConfig(CurrentSettings, path);
            ProfileChanged?.Invoke();
        }

        public static void SetActiveProfile(string name)
        {
            if (AppSettingsManager.CurrentSettings != null)
            {
                AppSettingsManager.CurrentSettings.ActiveProfileName = name;
                AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            }
            
            string path = Path.Combine(AppConfig.PakMasterConfigsFolder, name);
            CurrentSettings = LoadConfig(path) ?? new ToolConfigModel();
            ProfileChanged?.Invoke();
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
            string activeProfileName = AppSettingsManager.CurrentSettings?.ActiveProfileName ?? "tools-config.json";
            string path = customPath ?? Path.Combine(AppConfig.PakMasterConfigsFolder, activeProfileName);
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