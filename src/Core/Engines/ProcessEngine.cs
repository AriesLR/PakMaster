namespace PakMaster.Core.Engines
{
    public static class ProcessEngine
    {
        public static async Task RunToolAsync(string toolFolderName, string executableName, string arguments, Action<string> outputCallback)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string toolDirectory = Path.Combine(currentDirectory, "bin", toolFolderName);
                string executablePath = Path.Combine(toolDirectory, executableName);

                if (!Directory.Exists(toolDirectory))
                {
                    throw new DirectoryNotFoundException($"Tool directory not found: {toolDirectory}");
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = toolDirectory
                };

                await RunProcessCoreAsync(processStartInfo, outputCallback);
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        public static async Task RunUnrealPakAsync(string unrealPakPath, string arguments, Action<string> outputCallback)
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

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = unrealPakPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                };

                await RunProcessCoreAsync(processStartInfo, outputCallback);
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        private static async Task RunProcessCoreAsync(ProcessStartInfo processStartInfo, Action<string> outputCallback)
        {
            using (Process process = new Process { StartInfo = processStartInfo })
            {
                StringBuilder outputBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        outputBuilder.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        outputBuilder.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await Task.Run(() => process.WaitForExit());

                outputCallback?.Invoke(outputBuilder.ToString());
            }
        }
    }
}