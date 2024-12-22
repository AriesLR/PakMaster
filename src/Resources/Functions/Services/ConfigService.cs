using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace PakMaster.Resources.Functions.Services
{
    public class ConfigService
    {
        private readonly string _repakConfigFilePath;
        private readonly string _zenToolsConfigFilePath;

        public ConfigService()
        {
            // Get the path of the 'configs' folder next to the app
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string configsDirectory = Path.Combine(appDirectory, "configs");

            // Ensure the 'configs' folder exists
            if (!Directory.Exists(configsDirectory))
            {
                Directory.CreateDirectory(configsDirectory);
            }

            // Define the full paths of the config files
            _repakConfigFilePath = Path.Combine(configsDirectory, "repak-aeskey.json");
            _zenToolsConfigFilePath = Path.Combine(configsDirectory, "zentools-aeskey.json");
        }

        // Ensure Repak Config Exists
        public void EnsureRepakConfigExists()
        {
            if (!File.Exists(_repakConfigFilePath))
            {
                CreateDefaultRepakConfig();
            }
        }

        // Load Repak Config
        public T LoadRepakConfig<T>() where T : new()
        {
            return LoadConfig<T>(_repakConfigFilePath);
        }

        // Save Repak Config
        public void SaveRepakConfig<T>(T config)
        {
            SaveConfig(_repakConfigFilePath, config);
        }

        // Create Default Repak Config
        private void CreateDefaultRepakConfig()
        {
            var defaultConfig = new { AesKey = "" };
            SaveRepakConfig(defaultConfig);

            Debug.WriteLine("[DEBUG]: Default repak config created.");
        }

        // Ensure ZenTools Config Exists
        public void EnsureZenToolsConfigExists()
        {
            if (!File.Exists(_zenToolsConfigFilePath))
            {
                CreateDefaultZenToolsConfig();
            }
        }

        // Save ZenTools Config
        public void SaveZenToolsConfig(string guid, string aesKey)
        {
            try
            {
                var zenToolsConfig = new Dictionary<string, string>
        {
            { guid, aesKey }
        };

                string json = JsonConvert.SerializeObject(zenToolsConfig, Formatting.Indented);
                File.WriteAllText(_zenToolsConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG]: Error saving ZenTools config: {ex.Message}");
            }
        }

        // Load ZenTools Config
        public Dictionary<string, string> LoadZenToolsConfig()
        {
            return LoadConfig<Dictionary<string, string>>(_zenToolsConfigFilePath);
        }

        // Create Default ZenTools Config
        private void CreateDefaultZenToolsConfig()
        {
            string defaultGuid = "00000000-0000-0000-0000-000000000000";
            string defaultAesKey = string.Empty;

            var defaultConfig = new Dictionary<string, string>
    {
        { defaultGuid, defaultAesKey }
    };

            SaveConfig(_zenToolsConfigFilePath, defaultConfig);

            Debug.WriteLine("[DEBUG]: Default ZenTools config created.");
        }


        // Load Config
        private T LoadConfig<T>(string filePath) where T : new()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    Debug.WriteLine($"[DEBUG]: Config file not found at {filePath}. Creating a default one.");
                    return new T();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG]: Error loading config from {filePath}: {ex.Message}");
                return new T();
            }
        }

        // Save Config
        private void SaveConfig<T>(string filePath, T config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG]: Error saving config to {filePath}: {ex.Message}");
            }
        }
    }
}
