namespace PakMaster.Core.Models
{
    public class ToolConfigModel
    {
        public RepakSettingsModel Repak { get; set; } = new();
        public UnrealPakSettingsModel UnrealPak { get; set; } = new();
    }

    public class RepakVersionInfoModel
    {
        public string UEVersion { get; set; }
        public string Version { get; set; }
        public string VersionFeature { get; set; }
        public string Read { get; set; }
        public string Write { get; set; }
    }

    public class RepakSettingsModel
    {
        public string RepakVersion { get; set; } = "V11";
        public string AesKey { get; set; } = string.Empty;
    }

    public class UnrealPakSettingsModel
    {
        public string UnrealPakPath { get; set; } = string.Empty;
        public string GlobalOutputPath { get; set; } = string.Empty;
        public string CookedFilesPath { get; set; } = string.Empty;
        public string PackageStorePath { get; set; } = string.Empty;
        public string ScriptObjectsPath { get; set; } = string.Empty;
        public string IoStoreCommandsPath { get; set; } = string.Empty;
    }

}
    public class UnrealPakCryptoModel
    {
        public EncryptionKeyModel EncryptionKey { get; set; } = new();
        public string? SigningKey { get; set; }
        public bool bEnablePakSigning { get; set; } = false;
        public bool bEnablePakIndexEncryption { get; set; } = false;
        public bool bEnablePakIniEncryption { get; set; } = false;
        public bool bEnablePakUAssetEncryption { get; set; } = false;
        public bool bEnablePakFullAssetEncryption { get; set; } = false;
        public bool bDataCryptoRequired { get; set; } = true;
        public bool PakEncryptionRequired { get; set; } = true;
        public bool PakSigningRequired { get; set; } = true;
        public object? SecondaryEncryptionKeys { get; set; } = null;
    }

    public class EncryptionKeyModel
    {
        public string? Name { get; set; }
        public string? Guid { get; set; }
        public string? Key { get; set; }
    }
