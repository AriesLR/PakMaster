namespace PakMaster.Infrastructure.Platform
{
    public static class DependencyManager
    {
        private static readonly HttpClient _httpClient = new();

        private static readonly JsonSerializerOptions IndentedJsonOptions = new()
        {
            WriteIndented = true
        };

        // Process Result Enum
        public enum ProcessResult
        {
            Success,
            SuccessRebootRequired,
            UserCancelled,
            InstallerLocked,
            PlatformUnsupported,
            Failed
        }

        // Initialize Dependency Service
        public static async Task InitializeAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                string[]? names = DependencyConfig.Names;
                string[]? targets = DependencyConfig.DownloadUrls;
                string[]? arguments = DependencyConfig.Args;

                bool hasTargets = targets != null && targets.Length > 0;
                bool hasNames = names != null && names.Length > 0;

                if (hasTargets != hasNames)
                {
                    GLogger.Here().Error("Both DownloadTargets and Names must be populated together, or both left empty.");
                    return;
                }

                if (!hasTargets)
                {
                    GLogger.Here().Debug("No software dependencies are defined. Skipping dependency install pipeline.");
                    return;
                }

                if (names!.Length != targets!.Length)
                {
                    GLogger.Here().Error("The number of defined tracking Names ({NamesCount}) does not match the number of Download URLs ({TargetsCount}). Every URL needs a matching Key.", names.Length, targets.Length);
                    return;
                }

                var installedNames = LoadDependencyTracking();

                var pendingDeps = new List<int>();
                for (int i = 0; i < targets.Length; i++)
                {
                    string name = names[i];
                    if (!installedNames.Contains(name))
                    {
                        pendingDeps.Add(i);
                    }
                }

                if (pendingDeps.Count == 0)
                {
                    GLogger.Here().Debug("All configured dependencies are already marked as successfully installed.");
                    return;
                }

                GLogger.Here().Information("Discovered {Count} missing dependencies requiring install.", pendingDeps.Count);

                stopwatch.Stop();
                bool userConsented = await MessageManager.ShowYesNo(Lang.DependencyService_Msg_DepsFound_Title, Lang.DependencyService_Msg_DepsFound_Desc);
                stopwatch.Start();

                if (!userConsented)
                {
                    GLogger.Here().Warning("User declined dependency installations. Application termination sequence initiated.");
                    await Application.Current.Dispatcher.InvokeSafeAsync(() => Application.Current.Shutdown());
                    return;
                }

                await DownloadDependenciesAsync(pendingDeps, names, targets, arguments);
            }
            finally
            {
                stopwatch.Stop();
                GLogger.Here().Information("Initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        // Download Dependencies
        private static async Task DownloadDependenciesAsync(List<int> deps, string[] names, string[] targets, string[] arguments)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), AppConfig.AppName, "Deps");
            bool systemRebootRequested = false;

            try
            {
                if (!Directory.Exists(tempDirectory)) Directory.CreateDirectory(tempDirectory);

                foreach (int i in deps)
                {
                    string url = targets[i];
                    string name = (names != null && names.Length > i) ? names[i] : $"dep_{i}";
                    string? args = (arguments != null && arguments.Length > i) ? arguments[i] : null;

                    GLogger.Here().Information("Processing dependency [{Current}/{Total}]: {Name}", i + 1, targets.Length, name);

                    string fileName = Path.GetFileName(new Uri(url).LocalPath);
                    if (string.IsNullOrEmpty(fileName)) fileName = $"{name}_setup.exe";
                    string destinationPath = Path.Combine(tempDirectory, fileName);

                    GLogger.Here().Debug("Downloading dependency to: {Path}", destinationPath);

                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                    }

                    ProcessResult result = await RunDependencyProcess(destinationPath, args, name);

                    if (result == ProcessResult.Success || result == ProcessResult.SuccessRebootRequired)
                    {
                        if (result == ProcessResult.SuccessRebootRequired)
                        {
                            systemRebootRequested = true;
                        }

                        SaveDependencyAsInstalled(name);
                    }
                    else if (result == ProcessResult.UserCancelled)
                    {
                        GLogger.Here().Warning("Installation sequence halted because the user cancelled {Name}.", name);
                        await Application.Current.Dispatcher.InvokeSafeAsync(() => Application.Current.Shutdown());
                        return;
                    }
                    else if (result == ProcessResult.InstallerLocked)
                    {
                        string messageDesc = string.Format(Lang.DependencyService_Msg_InstallationBlocked_Desc, AppConfig.DisplayAppName);

                        await MessageManager.ShowOk(Lang.DependencyService_Msg_InstallationBlocked_Title, messageDesc);
                        await Application.Current.Dispatcher.InvokeSafeAsync(() => Application.Current.Shutdown());
                        return;
                    }
                    else if (result == ProcessResult.PlatformUnsupported)
                    {
                        string messageDesc = string.Format(Lang.DependencyService_Msg_IncompatibleDependency_Desc, name);

                        await MessageManager.ShowOk(Lang.DependencyService_Msg_IncompatibleDependency_Title, messageDesc);
                        await Application.Current.Dispatcher.InvokeSafeAsync(() => Application.Current.Shutdown());
                        return;
                    }
                    else
                    {
                        GLogger.Here().Error("Dependency installation encountered a fatal failure for {Name}. Halting sequence execution.", name);
                        await Application.Current.Dispatcher.InvokeSafeAsync(() => Application.Current.Shutdown());
                        return;
                    }
                }

                GLogger.Here().Information("All system dependencies successfully updated and configured.");

                if (systemRebootRequested)
                {
                    GLogger.Here().Information("Prompting user for system reboot authorization.");
                    bool systemRestartConsented = await MessageManager.ShowYesNo(Lang.DependencyService_Msg_RestartRequired_Title, Lang.DependencyService_Msg_RestartRequired_Desc);

                    if (systemRestartConsented)
                    {
                        GLogger.Here().Information("User approved system reboot. Executing OS shutdown.");
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "shutdown",
                            Arguments = "/r /t 5",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });

                        await Application.Current.Dispatcher.InvokeSafeAsync(() => Application.Current.Shutdown());
                        return;
                    }
                    else
                    {
                        GLogger.Here().Warning("User declined immediate system reboot. Running application anyway.");
                    }
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Fatal(ex, "An unhandled exception interrupted the dependency installer.");
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    try
                    {
                        Directory.Delete(tempDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        GLogger.Here().Warning(ex, "Failed to clean up temporary dependency directory: {Path}", tempDirectory);
                    }
                }
            }
        }

        // Run Dependency Process
        private static Task<ProcessResult> RunDependencyProcess(string executablePath, string? arguments, string name)
        {
            var tcs = new TaskCompletionSource<ProcessResult>();

            var processStartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                UseShellExecute = true,
                Verb = "runas"
            };

            var process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };

            GLogger.Here().Debug("Starting dependency installer process. Path: {Path} Args: {Args}", executablePath, arguments ?? "None");

            process.Exited += (sender, args) =>
            {
                int exitCode = process.ExitCode;
                process.Dispose();

                if (MsiErrorRegistry.Errors.TryGetValue(exitCode, out var errorInfo))
                {
                    if (errorInfo.Category == ProcessResult.Success)
                    {
                        GLogger.Here().Information("Dependency installation successfully finalized for: {Name} ({MsiCode}). Details: {Details}", name, errorInfo.Name, errorInfo.Description);
                    }
                    else if (errorInfo.Category == ProcessResult.SuccessRebootRequired)
                    {
                        GLogger.Here().Warning("Dependency installer {Name} completed successfully ({MsiCode}), but requires a system reboot. Details: {Details}", name, errorInfo.Name, errorInfo.Description);
                    }
                    else
                    {
                        GLogger.Here().Error("Dependency installer {Name} halted execution. Code: {Code} ({MsiCode}) -> {Details}", name, exitCode, errorInfo.Name, errorInfo.Description);
                    }

                    tcs.SetResult(errorInfo.Category);
                }
                else
                {
                    GLogger.Here().Error("Dependency installer {Name} terminated with an unhandled non-standard exit code: {ExitCode}", name, exitCode);
                    tcs.SetResult(ProcessResult.Failed);
                }
            };

            try
            {
                GLogger.Here().Debug("Launching setup tracking instance for: {Name}", name);
                process.Start();
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to initialize or launch installation for: {Name}", name);
                process.Dispose(); // Prevent native handle leak
                tcs.SetResult(ProcessResult.Failed);
            }

            return tcs.Task;
        }

        // Load Dependency Tracking
        private static HashSet<string> LoadDependencyTracking()
        {
            try
            {
                if (!File.Exists(AppConfig.AppDependencyTrackingPath)) return [];

                string jsonContent = File.ReadAllText(AppConfig.AppDependencyTrackingPath);
                var stateList = JsonSerializer.Deserialize<List<DependencyTrackStateModel>>(jsonContent);

                if (stateList == null) return [];

                return [.. stateList.Select(s => s.Key)];
            }
            catch (Exception ex)
            {
                GLogger.Here().Warning(ex, "Could not parse dependency tracking JSON. Defaulting to empty.");
                return [];
            }
        }

        // Save Dependency As Installed
        private static void SaveDependencyAsInstalled(string name)
        {
            try
            {
                string? directory = Path.GetDirectoryName(AppConfig.AppDependencyTrackingPath);
                if (directory != null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

                List<DependencyTrackStateModel> currentRecords = [];
                if (File.Exists(AppConfig.AppDependencyTrackingPath))
                {
                    string jsonContent = File.ReadAllText(AppConfig.AppDependencyTrackingPath);

                    currentRecords = JsonSerializer.Deserialize<List<DependencyTrackStateModel>>(jsonContent) ?? [];
                }

                currentRecords.Add(new DependencyTrackStateModel { Key = name, InstalledAt = DateTime.UtcNow });

                string updatedJson = JsonSerializer.Serialize(currentRecords, IndentedJsonOptions);
                File.WriteAllText(AppConfig.AppDependencyTrackingPath, updatedJson);

                GLogger.Here().Debug("Dependency name '{Name}' successfully tracked as installed.", name);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to persist dependency tracking state to disk for: {Name}", name);
            }
        }
    }
}