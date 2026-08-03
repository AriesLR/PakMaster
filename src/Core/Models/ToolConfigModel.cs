namespace PakMaster.Core.Models
{
    public class ToolConfigModel
    {
        public RepakInfoConfig RepakInfo { get; set; } = new();
        public RepakListConfig RepakList { get; set; } = new();
        public RepakHashListConfig RepakHashList { get; set; } = new();
        public RepakUnpackConfig RepakUnpack { get; set; } = new();
        public RepakPackConfig RepakPack { get; set; } = new();
        public RepakGetConfig RepakGet { get; set; } = new();
        public string ActiveRepakCommand { get; set; } = "info";
        public string ActiveRepakBranch { get; set; } = "main";

        public string ActiveRetocCommand { get; set; } = "manifest";
        public RetocManifestConfig RetocManifest { get; set; } = new();
        public RetocInfoConfig RetocInfo { get; set; } = new();
        public RetocListConfig RetocList { get; set; } = new();
        public RetocVerifyConfig RetocVerify { get; set; } = new();
        public RetocUnpackConfig RetocUnpack { get; set; } = new();
        public RetocUnpackRawConfig RetocUnpackRaw { get; set; } = new();
        public RetocPackRawConfig RetocPackRaw { get; set; } = new();
        public RetocToLegacyConfig RetocToLegacy { get; set; } = new();
        public RetocToZenConfig RetocToZen { get; set; } = new();
        public RetocGetConfig RetocGet { get; set; } = new();
        public RetocDumpTestConfig RetocDumpTest { get; set; } = new();
        public RetocGenScriptObjectsConfig RetocGenScriptObjects { get; set; } = new();
        public RetocPrintScriptObjectsConfig RetocPrintScriptObjects { get; set; } = new();
        public RetocAssetRegistryConfig RetocAssetRegistry { get; set; } = new();
    }

    public class RepakVersionInfoModel
    {
        public string UEVersion { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string VersionFeature { get; set; } = string.Empty;
        public string Read { get; set; } = string.Empty;
        public string Write { get; set; } = string.Empty;
    }

    public class RepakInfoConfig
    {
        public string TargetOperation { get; set; } = "info";
        public string InputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
    }

    public class RepakListConfig
    {
        public string TargetOperation { get; set; } = "list";
        public string InputPath { get; set; } = string.Empty;
        public string StripPrefix { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
    }

    public class RepakHashListConfig
    {
        public string TargetOperation { get; set; } = "hash-list";
        public string InputPath { get; set; } = string.Empty;
        public string StripPrefix { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
    }

    public class RepakUnpackConfig
    {
        public string TargetOperation { get; set; } = "unpack";
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string StripPrefix { get; set; } = string.Empty;
        public bool Verbose { get; set; } = false;
        public bool Quiet { get; set; } = false;
        public bool Force { get; set; } = false;
        public string Include { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
    }

    public class RepakPackConfig
    {
        public string TargetOperation { get; set; } = "pack";
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string MountPoint { get; set; } = string.Empty;
        public string RepakVersion { get; set; } = string.Empty;
        public string Compression { get; set; } = string.Empty;
        public string PathHashSeed { get; set; } = string.Empty;
        public bool Verbose { get; set; } = false;
        public bool Quiet { get; set; } = false;
        public string AesKey { get; set; } = string.Empty;
    }

    public class RepakGetConfig
    {
        public string TargetOperation { get; set; } = "get";
        public string InputPath { get; set; } = string.Empty;
        public string GetFile { get; set; } = string.Empty;
        public string StripPrefix { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
    }

    public class RetocManifestConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocInfoConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocListConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public bool All { get; set; } = false;
        public bool Hash { get; set; } = false;
        public bool Package { get; set; } = false;
        public bool Size { get; set; } = false;
        public bool Path { get; set; } = false;
        public bool Store { get; set; } = false;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocVerifyConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocUnpackConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public bool Verbose { get; set; } = false;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocUnpackRawConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocPackRawConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocToLegacyConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;
        public bool NoAssets { get; set; } = false;
        public bool NoShaders { get; set; } = false;
        public bool NoScriptObjects { get; set; } = false;
        public bool NoCompressShaders { get; set; } = false;
        public bool DryRun { get; set; } = false;
        public string EngineVersion { get; set; } = string.Empty;
        public string ScriptCell { get; set; } = string.Empty;
        public bool Verbose { get; set; } = false;
        public bool Debug { get; set; } = false;
        public bool NoParallel { get; set; } = false;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocToZenConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;
        public string EngineVersion { get; set; } = string.Empty;
        public string ScriptCell { get; set; } = string.Empty;
        public bool Verbose { get; set; } = false;
        public bool Debug { get; set; } = false;
        public bool NoParallel { get; set; } = false;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocGetConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string ChunkId { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocDumpTestConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string PackageId { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocGenScriptObjectsConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string EngineVersion { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocPrintScriptObjectsConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }

    public class RetocAssetRegistryConfig
    {
        public string InputPath { get; set; } = string.Empty;
        public string AesKey { get; set; } = string.Empty;
        public string OverrideContainerHeaderVersion { get; set; } = string.Empty;
        public string OverrideTocVersion { get; set; } = string.Empty;
    }
}