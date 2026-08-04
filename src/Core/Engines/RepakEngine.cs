namespace PakMaster.Core.Engines
{
    public static class RepakEngine
    {
        public static async Task ExecuteCommandAsync(string commandString, Action<string> outputCallback, CancellationToken ct = default)
        {
            GLogger.Here().Information("Starting Repak command execution: {CommandString}", commandString);
            if (string.IsNullOrWhiteSpace(commandString))
            {
                await MessageManager.ShowWarning(Lang.CommandCannotBeEmpty);
                return;
            }

            if (commandString.StartsWith("repak.exe", StringComparison.OrdinalIgnoreCase))
            {
                commandString = commandString["repak.exe".Length..].TrimStart();
            }

            string branchName = ConfigManager.CurrentSettings?.ActiveRepakBranch ?? "main";
            string exeName = "repak.exe";

            if (!string.IsNullOrWhiteSpace(branchName) && branchName != "main")
            {
                exeName = $"repak-{branchName}.exe";
            }

            await ProcessEngine.RunToolAsync("repak", exeName, commandString, outputCallback, ct);
        }

        public static string BuildCommandString(ToolConfigModel settings)
        {
            if (settings == null) return "repak.exe";

            string cmd = settings.ActiveRepakCommand ?? "unpack";
            string finalCmd = "repak.exe";

            string aesKey = string.Empty;
            string inputPath = string.Empty;
            string outputPath = string.Empty;
            string stripPrefix = string.Empty;
            bool verbose = false;
            bool quiet = false;

            switch (cmd)
            {
                case "info":
                    aesKey = settings.RepakInfo.AesKey;
                    inputPath = settings.RepakInfo.InputPath;
                    break;
                case "list":
                    aesKey = settings.RepakList.AesKey;
                    inputPath = settings.RepakList.InputPath;
                    stripPrefix = settings.RepakList.StripPrefix;
                    break;
                case "hash-list":
                    aesKey = settings.RepakHashList.AesKey;
                    inputPath = settings.RepakHashList.InputPath;
                    stripPrefix = settings.RepakHashList.StripPrefix;
                    break;
                case "unpack":
                    aesKey = settings.RepakUnpack.AesKey;
                    inputPath = settings.RepakUnpack.InputPath;
                    outputPath = settings.RepakUnpack.OutputPath;
                    stripPrefix = settings.RepakUnpack.StripPrefix;
                    verbose = settings.RepakUnpack.Verbose;
                    quiet = settings.RepakUnpack.Quiet;
                    break;
                case "pack":
                    aesKey = settings.RepakPack.AesKey;
                    inputPath = settings.RepakPack.InputPath;
                    outputPath = settings.RepakPack.OutputPath;
                    verbose = settings.RepakPack.Verbose;
                    quiet = settings.RepakPack.Quiet;
                    break;
                case "get":
                    aesKey = settings.RepakGet.AesKey;
                    inputPath = settings.RepakGet.InputPath;
                    stripPrefix = settings.RepakGet.StripPrefix;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(aesKey)) finalCmd += $" --aes-key {aesKey}";
            finalCmd += $" {cmd}";

            bool isPack = cmd == "pack";
            bool isUnpack = cmd == "unpack";
            bool isGet = cmd == "get";

            if (isPack)
            {
                if (!string.IsNullOrWhiteSpace(settings.RepakPack.MountPoint)) finalCmd += $" --mount-point \"{settings.RepakPack.MountPoint}\"";
                if (!string.IsNullOrWhiteSpace(settings.RepakPack.RepakVersion)) finalCmd += $" --version {settings.RepakPack.RepakVersion}";
                if (!string.IsNullOrWhiteSpace(settings.RepakPack.Compression)) finalCmd += $" --compression {settings.RepakPack.Compression}";
                if (!string.IsNullOrWhiteSpace(settings.RepakPack.PathHashSeed)) finalCmd += $" --path-hash-seed {settings.RepakPack.PathHashSeed}";
                if (verbose) finalCmd += " --verbose";
                if (quiet) finalCmd += " --quiet";
            }
            else if (isUnpack)
            {
                string modifiedOutputPath = outputPath;
                if (!string.IsNullOrWhiteSpace(inputPath) && !string.IsNullOrWhiteSpace(modifiedOutputPath))
                {
                    string pakName = Path.GetFileNameWithoutExtension(inputPath);
                    modifiedOutputPath = Path.Combine(modifiedOutputPath, pakName);
                }

                if (!string.IsNullOrWhiteSpace(modifiedOutputPath)) finalCmd += $" --output \"{modifiedOutputPath}\"";
                if (!string.IsNullOrWhiteSpace(stripPrefix)) finalCmd += $" --strip-prefix \"{stripPrefix}\"";
                if (settings.RepakUnpack.Force) finalCmd += " --force";
                if (!string.IsNullOrWhiteSpace(settings.RepakUnpack.Include))
                {
                    var includes = settings.RepakUnpack.Include.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var inc in includes)
                    {
                        finalCmd += $" --include \"{inc.Trim()}\"";
                    }
                }
                if (verbose) finalCmd += " --verbose";
                if (quiet) finalCmd += " --quiet";
            }
            else if (cmd == "list" || cmd == "hash-list" || cmd == "get")
            {
                if (!string.IsNullOrWhiteSpace(stripPrefix)) finalCmd += $" --strip-prefix \"{stripPrefix}\"";
            }

            if (!string.IsNullOrWhiteSpace(inputPath)) finalCmd += $" \"{inputPath}\"";

            if (isGet && !string.IsNullOrWhiteSpace(settings.RepakGet.GetFile))
            {
                finalCmd += $" \"{settings.RepakGet.GetFile}\"";
            }
            else if (isPack && !string.IsNullOrWhiteSpace(outputPath))
            {
                string modifiedOutputPath = outputPath;
                if (Directory.Exists(modifiedOutputPath))
                {
                    if (!string.IsNullOrWhiteSpace(inputPath))
                    {
                        string dirName = Path.GetFileName(inputPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                        modifiedOutputPath = Path.Combine(modifiedOutputPath, dirName + ".pak");
                    }
                }
                finalCmd += $" \"{modifiedOutputPath}\"";
            }

            return finalCmd;
        }
    }
}