using System;
using System.IO;
using System.Text.Json;
using PakMaster.Core.Models;
using PakMaster.Infrastructure.Diagnostics;

namespace PakMaster.Core.Settings
{
    public static class ConfigManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
        private static readonly System.Threading.SemaphoreSlim _saveLock = new(1, 1);

        public static ToolConfigModel CurrentSettings { get; set; } = new ToolConfigModel();
        
        public static string ToolConfigPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs", "pakmaster-tools-config.json");
        public static string ZenToolsConfigPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs", "zentools-aeskey.json");
        public static string CryptoConfigPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs", "Crypto.json");

        public static void Initialize()
        {
            CurrentSettings = LoadConfig() ?? new ToolConfigModel();
            EnsureConfigsExist();
        }

        private static void EnsureConfigsExist()
        {
            try
            {
                string? directory = Path.GetDirectoryName(CryptoConfigPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(CryptoConfigPath))
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
                    File.WriteAllText(CryptoConfigPath, json);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to create default Crypto.json");
            }

            try
            {
                string? directory = Path.GetDirectoryName(ZenToolsConfigPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(ZenToolsConfigPath))
                {
                    var defaultZen = new System.Collections.Generic.Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "" } };
                    string json = JsonSerializer.Serialize(defaultZen, JsonOptions);
                    File.WriteAllText(ZenToolsConfigPath, json);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to create default zentools-aeskey.json");
            }
        }

        public static ToolConfigModel? LoadConfig(string? customPath = null)
        {
            string path = customPath ?? ToolConfigPath;
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
            string path = customPath ?? ToolConfigPath;
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
    
        public static System.Collections.Generic.Dictionary<string, string>? LoadZenToolsConfig()
        {
            try
            {
                if (File.Exists(ZenToolsConfigPath))
                {
                    string json = File.ReadAllText(ZenToolsConfigPath);
                    return JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(json);
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
                var dict = new System.Collections.Generic.Dictionary<string, string> { { guid, hex } };
                string json = JsonSerializer.Serialize(dict, JsonOptions);
                File.WriteAllText(ZenToolsConfigPath, json);
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
                if (File.Exists(CryptoConfigPath))
                {
                    string json = File.ReadAllText(CryptoConfigPath);
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
                File.WriteAllText(CryptoConfigPath, json);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Error saving UnrealPak crypto config");
            }
        }
}
}
