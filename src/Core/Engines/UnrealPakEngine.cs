namespace PakMaster.Core.Engines
{
    public static class UnrealPakEngine
    {
        public static async Task RepackAsync(Action<string> outputCallback)
        {
            var unrealPakConfig = ConfigManager.CurrentSettings.UnrealPak;
            string unrealPakPath = unrealPakConfig.UnrealPakPath;
            string globalOutputPath = unrealPakConfig.GlobalOutputPath;
            string cookedFilesPath = unrealPakConfig.CookedFilesPath;
            string packageStorePath = unrealPakConfig.PackageStorePath;
            string scriptObjectsPath = unrealPakConfig.ScriptObjectsPath;
            string ioStoreCommandsPath = unrealPakConfig.IoStoreCommandsPath;

            if (string.IsNullOrEmpty(unrealPakPath) || !File.Exists(unrealPakPath))
            {
                await MessageManager.ShowWarning("UnrealPak executable path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(globalOutputPath))
            {
                await MessageManager.ShowWarning("Please specify an output path.");
                return;
            }

            if (string.IsNullOrEmpty(cookedFilesPath) || !Directory.Exists(cookedFilesPath))
            {
                await MessageManager.ShowWarning("Cooked files path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(packageStorePath) || !File.Exists(packageStorePath))
            {
                await MessageManager.ShowWarning("PackageStore.manifest path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(scriptObjectsPath) || !File.Exists(scriptObjectsPath))
            {
                await MessageManager.ShowWarning("ScriptObjects.bin path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(ioStoreCommandsPath) || !File.Exists(ioStoreCommandsPath))
            {
                await MessageManager.ShowWarning("IoStoreCommands.txt path is missing or invalid.");
                return;
            }

            string cryptoKeysPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs", "Crypto.json");

            string finalGlobalOutputPath = Path.Combine(globalOutputPath, "global.utoc");

            string arguments = $"-CreateGlobalContainer=\"{finalGlobalOutputPath}\" " +
                               $"-CookedDirectory=\"{cookedFilesPath}\" " +
                               $"-WriteBackMetadataToAssetRegistry=Disabled " +
                               $"-PackageStoreManifest=\"{packageStorePath}\" " +
                               $"-Commands=\"{ioStoreCommandsPath}\" " +
                               $"-ScriptObjects=\"{scriptObjectsPath}\" " +
                               $"-patchpaddingalign=2048 " +
                               $"-compressionformats=Oodle " +
                               $"-compresslevel=4 " +
                               $"-compressionmethod=Kraken " +
                               $"-cryptokeys=\"{cryptoKeysPath}\" " +
                               $"-compressionMinBytesSaved=1024 " +
                               $"-compressionMinPercentSaved=5";

            GLogger.Here().Debug($"[DEBUG]: UnrealPak Configuration Loaded:");
            GLogger.Here().Debug($"[DEBUG]: UnrealPak Path: {unrealPakPath}");
            GLogger.Here().Debug($"[DEBUG]: Output Path: {finalGlobalOutputPath}");
            GLogger.Here().Debug($"[DEBUG]: Cooked Files Path: {cookedFilesPath}");
            GLogger.Here().Debug($"[DEBUG]: PackageStore Path: {packageStorePath}");
            GLogger.Here().Debug($"[DEBUG]: IoStoreCommands Path: {ioStoreCommandsPath}");
            GLogger.Here().Debug($"[DEBUG]: ScriptObjects Path: {scriptObjectsPath}");
            GLogger.Here().Debug($"[DEBUG]: Crypto Keys Path: {cryptoKeysPath}");
            GLogger.Here().Debug($"[DEBUG]: Arguments: {arguments}");

            await ProcessEngine.RunUnrealPakAsync(unrealPakPath, arguments, outputCallback);
        }
    }
}