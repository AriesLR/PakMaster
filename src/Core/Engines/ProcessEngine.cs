namespace PakMaster.Core.Engines
{
    public static class ProcessEngine
    {
        public static async Task RunToolAsync(string toolFolderName, string executableName, IEnumerable<string> arguments, Action<string> outputCallback, CancellationToken ct = default)
        {
            try
            {
                string toolDirectory = Path.Combine(AppConfig.PakMasterDependencyFolder, toolFolderName);
                string executablePath = Path.Combine(toolDirectory, executableName);

                if (!Directory.Exists(toolDirectory))
                {
                    throw new DirectoryNotFoundException($"Tool directory not found: {toolDirectory}");
                }

                await RunProcessCoreAsync(executablePath, toolDirectory, arguments, outputCallback, ct);
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        public static async Task RunUnrealPakAsync(string unrealPakPath, IEnumerable<string> arguments, Action<string> outputCallback, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrEmpty(unrealPakPath))
                {
                    throw new ArgumentException("UnrealPak path is not provided.");
                }

                if (!File.Exists(unrealPakPath))
                {
                    throw new FileNotFoundException($"UnrealPak executable not found: {unrealPakPath}");
                }

                string workingDirectory = Path.GetDirectoryName(unrealPakPath) ?? string.Empty;
                if (string.IsNullOrEmpty(workingDirectory))
                {
                    throw new DirectoryNotFoundException("Could not determine the working directory for UnrealPak.");
                }

                await RunProcessCoreAsync(unrealPakPath, workingDirectory, arguments, outputCallback, ct);
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        private static async Task RunProcessCoreAsync(string executablePath, string workingDirectory, IEnumerable<string> arguments, Action<string> outputCallback, CancellationToken ct)
        {
            StringBuilder outputBuilder = new();
            object lockObj = new();

            var pipeTarget = PipeTarget.ToDelegate(
                line =>
                {
                    lock (lockObj)
                    {
                        outputBuilder.AppendLine(line);
                    }
                }
            );

            var cmd = Cli.Wrap(executablePath)
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(arguments)
                .WithValidation(CommandResultValidation.None)
                .WithStandardOutputPipe(pipeTarget)
                .WithStandardErrorPipe(pipeTarget);

            await cmd.ExecuteAsync(ct);

            outputCallback?.Invoke(outputBuilder.ToString());
        }
    }
}