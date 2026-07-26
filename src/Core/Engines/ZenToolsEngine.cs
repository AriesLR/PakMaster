namespace PakMaster.Core.Engines
{
    public static class ZenToolsEngine
    {
        public static async Task UnpackAsync(string inputFolderPath, string outputFolderPath, Action<string> outputCallback, CancellationToken ct = default)
        {
            var zentoolsConfig = ConfigManager.LoadZenToolsConfig();
            string zenToolsKeyGuid = string.Empty;
            string zenToolsKeyHex = string.Empty;
            if (zentoolsConfig != null)
            {
                foreach (var kvp in zentoolsConfig)
                {
                    zenToolsKeyGuid = kvp.Key;
                    zenToolsKeyHex = kvp.Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(zenToolsKeyGuid))
            {
                await MessageManager.ShowError("ZenTools AES Key (GUID) not found in the config.\n\nThe GUID cannot be left blank.\n\nDefault GUID: 00000000-0000-0000-0000-000000000000");
                return;
            }

            if (string.IsNullOrEmpty(zenToolsKeyHex))
            {
                GLogger.Here().Debug($"No ZenTools AES Key Hex Found.");
            }
            else
            {
                GLogger.Here().Debug($"ZenTools AES Key Found:\nGUID: {zenToolsKeyGuid}\nHex: {zenToolsKeyHex}");
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                await MessageManager.ShowWarning("Please select an input folder.");
                return;
            }

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                await MessageManager.ShowWarning("Please select an output folder.");
                return;
            }

            string inputPath = inputFolderPath;

            string uniqueGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
            string outputPath = Path.Combine(outputFolderPath, $"PakMaster_IoStore_{uniqueGuid}");

            var arguments = new List<string>
            {
                "ExtractPackages",
                inputPath,
                outputPath
            };

            if (!string.IsNullOrEmpty(zenToolsKeyHex))
            {
                arguments.Add($"-EncryptionKeys={AppConfig.ZenToolsConfigPath}");
            }

            arguments.Add("-ZenPackageVersion=Initial");

            await ProcessEngine.RunToolAsync("zentools", "zentools.exe", arguments, outputCallback, ct);

            string appDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)) ?? string.Empty;
            string engineFolderPath = Path.Combine(appDirectory, "Engine");
            string zenToolsFolderPath = Path.Combine(appDirectory, "ZenTools");

            try
            {
                if (Directory.Exists(engineFolderPath))
                {
                    Directory.Delete(engineFolderPath, true);
                    GLogger.Here().Debug($"Deleted folder: {engineFolderPath}");
                }

                if (Directory.Exists(zenToolsFolderPath))
                {
                    Directory.Delete(zenToolsFolderPath, true);
                    GLogger.Here().Debug($"Deleted folder: {zenToolsFolderPath}");
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Debug($"Failed to delete folders: {ex.Message}");
            }
        }
    }
}