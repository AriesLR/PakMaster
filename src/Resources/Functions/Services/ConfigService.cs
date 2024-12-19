using Newtonsoft.Json;
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

        // Method to check if the config exists and create it if not
        public void EnsureConfigExists()
        {
            if (!File.Exists(_configFilePath))
            {
                CreateDefaultConfig();
            }
        }

        // Method to load configuration
        public T LoadConfig<T>() where T : new()
        {
            try
            {
                string json = File.ReadAllText(_configFilePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                return new T(); // Return default config on error
            }
        }

        // Method to save configuration
        public void SaveConfig<T>(T config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }

        // Method to create a default config file
        private void CreateDefaultConfig()
        {
            try
            {
                var defaultConfig = new { AesKey = "" }; // Define default config structure
                SaveConfig(defaultConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating default config: {ex.Message}");
            }
        }
    }
}
