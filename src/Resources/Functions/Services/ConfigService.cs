﻿using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace PakMaster.Resources.Functions.Services
{
    public class ConfigService
    {
        private readonly string _generalConfigFilePath;
        private readonly string _repakConfigFilePath;
        private readonly string _zenToolsConfigFilePath;
        private readonly string _unrealpakConfigFilePath;
        private readonly string _unrealpakCryptoConfigFilePath;

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
            _generalConfigFilePath = Path.Combine(configsDirectory, "general-config.json"); // General Config [Repak/ZenTools Settings]
            _repakConfigFilePath = Path.Combine(configsDirectory, "repak-aeskey.json"); // Repak AES Key
            _zenToolsConfigFilePath = Path.Combine(configsDirectory, "zentools-aeskey.json"); // ZenTools AES Key
            _unrealpakConfigFilePath = Path.Combine(configsDirectory, "unrealpak-config.json"); // UnrealPak Config [File/Folder Paths]
            _unrealpakCryptoConfigFilePath = Path.Combine(configsDirectory, "Crypto.json"); // UnrealPak Crypto File  [Crypto File]
        }

        //////////////////////////
        // ENSURE CONFIGS EXIST //
        ////////////////////////// 

        public void EnsureConfigsExist()
        {
            if (!File.Exists(_repakConfigFilePath))
            {
                Debug.WriteLine("[DEBUG]: Repak config doesn't exist, creating one now.");
                CreateDefaultRepakConfig();
            }
            if (!File.Exists(_zenToolsConfigFilePath))
            {
                Debug.WriteLine("[DEBUG]: ZenTools config doesn't exist, creating one now.");
                CreateDefaultZenToolsConfig();
            }
            if (!File.Exists(_unrealpakConfigFilePath))
            {
                Debug.WriteLine("[DEBUG]: UnrealPak config doesn't exist, creating one now.");
                CreateDefaultUnrealPakConfig();
            }
            if (!File.Exists(_unrealpakCryptoConfigFilePath))
            {
                Debug.WriteLine("[DEBUG]: UnrealPak Crypto config doesn't exist, creating one now.");
                CreateDefaultUnrealPakCryptoConfig();
            }
            if (!File.Exists(_generalConfigFilePath))
            {
                Debug.WriteLine("[DEBUG]: General config doesn't exist, creating one now.");
                CreateDefaultGeneralConfig();
            }
        }

        ////////////////////////////
        // CREATE DEFAULT CONFIGS //
        //////////////////////////// 

        // Create Default General Config
        private void CreateDefaultGeneralConfig()
        {
            var defaultConfig = new
            {
                RepakVersion = "V11"
            };

            SaveConfig(_generalConfigFilePath, defaultConfig);

            Debug.WriteLine("[DEBUG]: Default General config created.");
        }

        // Create Default Repak Config
        private void CreateDefaultRepakConfig()
        {
            var defaultConfig = new { AesKey = "" };
            SaveRepakConfig(defaultConfig);

            Debug.WriteLine("[DEBUG]: Default repak config created.");
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

        // Create Default UnrealPak Config
        private void CreateDefaultUnrealPakConfig()
        {
            var defaultConfig = new
            {
                UnrealPakPath = string.Empty,
                GlobalOutputPath = string.Empty,
                CookedFilesPath = string.Empty,
                PackageStorePath = string.Empty,
                ScriptObjectsPath = string.Empty,
                IoStoreCommandsPath = string.Empty
            };

            SaveConfig(_unrealpakConfigFilePath, defaultConfig);

            Debug.WriteLine("[DEBUG]: Default UnrealPak config created.");
        }

        // Create Default UnrealPak Crypto Config
        private void CreateDefaultUnrealPakCryptoConfig()
        {
            var defaultCryptoConfig = new
            {
                EncryptionKey = new
                {
                    Name = (string?)null,
                    Guid = (string?)null,
                    Key = (string?)null
                },
                SigningKey = (string?)null,
                bEnablePakSigning = false,
                bEnablePakIndexEncryption = false,
                bEnablePakIniEncryption = false,
                bEnablePakUAssetEncryption = false,
                bEnablePakFullAssetEncryption = false,
                bDataCryptoRequired = true,
                PakEncryptionRequired = true,
                PakSigningRequired = true,
                SecondaryEncryptionKeys = (object?)null
            };

            SaveConfig(_unrealpakCryptoConfigFilePath, defaultCryptoConfig);

            Debug.WriteLine("[DEBUG]: Default UnrealPak Crypto config created.");
        }

        ///////////////////////////
        // LOAD SPECIFIC CONFIGS //
        /////////////////////////// 

        // Load General Config
        public T LoadGeneralConfig<T>() where T : new()
        {
            return LoadConfig<T>(_generalConfigFilePath);
        }

        // Load Repak Config
        public T LoadRepakConfig<T>() where T : new()
        {
            return LoadConfig<T>(_repakConfigFilePath);
        }

        // Load ZenTools Config
        public Dictionary<string, string> LoadZenToolsConfig()
        {
            return LoadConfig<Dictionary<string, string>>(_zenToolsConfigFilePath);
        }

        // Load UnrealPak Config
        public T LoadUnrealPakConfig<T>() where T : new()
        {
            return LoadConfig<T>(_unrealpakConfigFilePath);
        }

        // Load UnrealPak Crypto Config
        public T LoadUnrealPakCryptoConfig<T>() where T : new()
        {
            return LoadConfig<T>(_unrealpakCryptoConfigFilePath);
        }

        ///////////////////////////
        // SAVE SPECIFIC CONFIGS //
        /////////////////////////// 

        // Save General Config
        public void SaveGeneralConfig(object config)
        {
            SaveConfig(_generalConfigFilePath, config);
            Debug.WriteLine("[DEBUG]: General config saved.");
        }

        // Save Repak Config
        public void SaveRepakConfig<T>(T config)
        {
            SaveConfig(_repakConfigFilePath, config);
            Debug.WriteLine("[DEBUG]: Repak config saved.");
        }

        // Save ZenTools Config
        public void SaveZenToolsConfig(string guid, string aesKey)
        {
            var zenToolsConfig = new Dictionary<string, string>
            {
                { guid, aesKey }
            };
            SaveConfig(_zenToolsConfigFilePath, zenToolsConfig);
            Debug.WriteLine("[DEBUG]: ZenTools config saved.");
        }

        // Save UnrealPak Config
        public void SaveUnrealPakConfig(object config)
        {
            SaveConfig(_unrealpakConfigFilePath, config);
            Debug.WriteLine("[DEBUG]: UnrealPak config saved.");
        }

        // Save UnrealPak Crypto Config
        public void SaveUnrealPakCryptoConfig(object cryptoConfig)
        {
            SaveConfig(_unrealpakCryptoConfigFilePath, cryptoConfig);
            Debug.WriteLine("[DEBUG]: UnrealPak Crypto config saved.");
        }

        ///////////////////////
        // LOAD/SAVE HELPERS //
        ///////////////////////

        // Load Config Helper
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
                    return new T();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG]: Error loading config from {filePath}: {ex.Message}");
                return new T();
            }
        }

        // Save Config Helper
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
