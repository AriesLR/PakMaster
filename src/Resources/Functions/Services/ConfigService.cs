using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace PakMaster.Resources.Functions.Services
{
    public class ConfigService
    {
        private readonly string _configFilePath;

        public ConfigService(string configFileName = "config.json")
        {
            // Get the path of the 'configs' folder next to the app
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string configsDirectory = Path.Combine(appDirectory, "configs");

            // Ensure the 'configs' folder exists
            if (!Directory.Exists(configsDirectory))
            {
                Directory.CreateDirectory(configsDirectory);
            }

            // Define the full path of the config file
            _configFilePath = Path.Combine(configsDirectory, configFileName);
        }

        // Check if config exists, if not, create a config file
        public void EnsureConfigExists()
        {
            if (!File.Exists(_configFilePath))
            {
                CreateDefaultConfig();
            }
        }

        // Load Config
        public T LoadConfig<T>() where T : new()
        {
            try
            {
                string json = File.ReadAllText(_configFilePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG]: Error loading config: {ex.Message}");
                return new T(); // Return default config on error
            }
        }

        // Save Config
        public void SaveConfig<T>(T config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG]: Error saving config: {ex.Message}");
            }
        }

        // Create Default Config
        private void CreateDefaultConfig()
        {
            try
            {
                var defaultConfig = new { AesKey = "" };
                SaveConfig(defaultConfig);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG]: Error creating default config: {ex.Message}");
            }
        }
    }
}
