namespace PakMaster.Core.Engines
{
    public static class ProcessEngine
    {
        public static event Action<string>? OnCliProcessStarted;
        public static event Action? OnCliProcessEnded;
        public static event Action<string>? OnCliOutputLine;

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
                GLogger.Here().Error(ex, "Error running command"); await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        public static async Task RunToolAsync(string toolFolderName, string executableName, string arguments, Action<string> outputCallback, CancellationToken ct = default)
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
                GLogger.Here().Error(ex, "Error running command"); await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        private static async Task RunProcessCoreAsync(string executablePath, string workingDirectory, IEnumerable<string> arguments, Action<string> outputCallback, CancellationToken ct)
        {
            StringBuilder outputBuilder = new();
            object lockObj = new();

            GLogger.Here().Debug("Initializing command execution process"); 
            var pipeTarget = PipeTarget.ToDelegate(
                line =>
                {
                    lock (lockObj)
                    {
                        outputBuilder.AppendLine(line); 
                        GLogger.Here().Information("[{0}]: {1}", Path.GetFileName(executablePath), line);
                        OnCliOutputLine?.Invoke(line);
                    }
                }
            );

            var cmd = Cli.Wrap(executablePath)
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(arguments)
                .WithValidation(CommandResultValidation.None)
                .WithStandardOutputPipe(pipeTarget)
                .WithStandardErrorPipe(pipeTarget);

            OnCliProcessStarted?.Invoke(Path.GetFileName(executablePath));
            await cmd.ExecuteAsync(ct); 
            GLogger.Here().Information("Command execution finished successfully");
            OnCliProcessEnded?.Invoke();

            outputCallback?.Invoke(outputBuilder.ToString());
        }

        private static async Task RunProcessCoreAsync(string executablePath, string workingDirectory, string arguments, Action<string> outputCallback, CancellationToken ct)
        {
            StringBuilder outputBuilder = new();
            object lockObj = new();

            GLogger.Here().Debug("Initializing command execution process"); 
            var pipeTarget = PipeTarget.ToDelegate(
                line =>
                {
                    lock (lockObj)
                    {
                        outputBuilder.AppendLine(line); 
                        GLogger.Here().Information("[{0}]: {1}", Path.GetFileName(executablePath), line);
                        OnCliOutputLine?.Invoke(line);
                    }
                }
            );

            var cmd = Cli.Wrap(executablePath)
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(arguments)
                .WithValidation(CommandResultValidation.None)
                .WithStandardOutputPipe(pipeTarget)
                .WithStandardErrorPipe(pipeTarget);

            OnCliProcessStarted?.Invoke(Path.GetFileName(executablePath));
            await cmd.ExecuteAsync(ct); 
            GLogger.Here().Information("Command execution finished successfully");
            OnCliProcessEnded?.Invoke();

            outputCallback?.Invoke(outputBuilder.ToString());
        }
    }
}
