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
            EnsureConfigsExist();
        }

        private static void EnsureConfigsExist()
        {
            try
            {
                string? directory = Path.GetDirectoryName(AppConfig.CryptoConfigPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(AppConfig.CryptoConfigPath))
                {
                    var defaultCrypto = new UnrealPakCryptoModel
                    {
                        EncryptionKey = new EncryptionKeyModel { Name = null, Guid = null, Key = null },
                        SigningKey = null,
                        bEnablePakSigning = false,
                        bEnablePakIndexEncryption = false,
                        bEnablePakIniEncryption = false,
                        bEnablePakUAssetEncryption = false,
                        bEnablePakFullAssetEncryption = false,
                        bDataCryptoRequired = true,
                        PakEncryptionRequired = true,
                        PakSigningRequired = true,
                        SecondaryEncryptionKeys = null
                    };
                    string json = JsonSerializer.Serialize(defaultCrypto, JsonOptions);
                    File.WriteAllText(AppConfig.CryptoConfigPath, json);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to create default Crypto.json");
            }

            try
            {
                string? directory = Path.GetDirectoryName(AppConfig.ZenToolsConfigPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(AppConfig.ZenToolsConfigPath))
                {
                    var defaultZen = new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "" } };
                    string json = JsonSerializer.Serialize(defaultZen, JsonOptions);
                    File.WriteAllText(AppConfig.ZenToolsConfigPath, json);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to create default zentools-aeskey.json");
            }
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

        public static Dictionary<string, string>? LoadZenToolsConfig()
        {
            try
            {
                if (File.Exists(AppConfig.ZenToolsConfigPath))
                {
                    string json = File.ReadAllText(AppConfig.ZenToolsConfigPath);
                    return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Error loading ZenTools config");
            }
            return null;
        }

        public static void SaveZenToolsConfig(string guid, string hex)
        {
            try
            {
                var dict = new Dictionary<string, string> { { guid, hex } };
                string json = JsonSerializer.Serialize(dict, JsonOptions);
                File.WriteAllText(AppConfig.ZenToolsConfigPath, json);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Error saving ZenTools config");
            }
        }

        public static UnrealPakCryptoModel? LoadUnrealPakCryptoConfig()
        {
            try
            {
                if (File.Exists(AppConfig.CryptoConfigPath))
                {
                    string json = File.ReadAllText(AppConfig.CryptoConfigPath);
                    return JsonSerializer.Deserialize<UnrealPakCryptoModel>(json);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Error loading UnrealPak crypto config");
            }
            return null;
        }

        public static void SaveUnrealPakCryptoConfig(UnrealPakCryptoModel model)
        {
            try
            {
                string json = JsonSerializer.Serialize(model, JsonOptions);
                File.WriteAllText(AppConfig.CryptoConfigPath, json);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Error saving UnrealPak crypto config");
            }
        }
    }
}