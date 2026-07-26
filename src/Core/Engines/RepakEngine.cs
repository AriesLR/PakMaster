namespace PakMaster.Core.Engines
{
    public static class RepakEngine
    {
        public static async Task UnpackAsync(string fullInputFilePath, string outputFolderPath, Action<string> outputCallback, CancellationToken ct = default)
        {
            var config = ConfigManager.CurrentSettings;
            string aesKey = config.Repak.AesKey;

            if (string.IsNullOrEmpty(aesKey))
            {
                GLogger.Here().Debug("AES Key is empty");
            }
            else
            {
                GLogger.Here().Debug($"AES Key found.\nAES Key: {aesKey}");
            }

            if (string.IsNullOrEmpty(fullInputFilePath))
            {
                await MessageManager.ShowWarning("Invalid file path.");
                return;
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullInputFilePath);

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                await MessageManager.ShowWarning("Please select an output folder.");
                return;
            }

            string outputPath = Path.Combine(outputFolderPath, fileNameWithoutExtension);

            var arguments = new List<string>();
            if (!string.IsNullOrEmpty(aesKey))
            {
                arguments.Add("-a");
                arguments.Add(aesKey);
            }
            arguments.Add("unpack");
            arguments.Add("-o");
            arguments.Add(outputPath);
            arguments.Add(fullInputFilePath);

            await ProcessEngine.RunToolAsync("repak", "repak.exe", arguments, outputCallback, ct);
        }

        public static async Task RepackAsync(string fullInputFolderPath, string inputFolderPath, Action<string> outputCallback, CancellationToken ct = default)
        {
            var repakConfig = ConfigManager.CurrentSettings.Repak;
            string repakVersion = repakConfig.RepakVersion;

            if (!Directory.Exists(fullInputFolderPath))
            {
                await MessageManager.ShowWarning($"The selected folder does not exist: {fullInputFolderPath}");
                return;
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                await MessageManager.ShowWarning("Please browse and select an input folder first.");
                return;
            }

            string folderName = Path.GetFileName(fullInputFolderPath);

            if (string.IsNullOrEmpty(folderName))
            {
                await MessageManager.ShowWarning("Invalid input folder name.");
                return;
            }

            string outputPakName = folderName.EndsWith("_P")
                ? folderName.Substring(0, folderName.Length - 2) + "_Modified_P.pak"
                : folderName + "_Modified_P.pak";

            string outputFilePath = Path.Combine(inputFolderPath, outputPakName);

            var arguments = new List<string>
            {
                "pack",
                "--version",
                repakVersion,
                fullInputFolderPath,
                outputFilePath
            };

            await ProcessEngine.RunToolAsync("repak", "repak.exe", arguments, outputCallback, ct);
        }
    }
}