namespace PakMaster.Core.Engines
{
    public static class ToolDependencyEngine
    {
        // Method to check if dependencies exist in the specified subdirectory
        public static bool CheckIfDependencyExists(string subDirectory, string exeName)
        {
            string targetDirectory = Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory);
            string targetFilePath = Path.Combine(targetDirectory, exeName);

            return File.Exists(targetFilePath);
        }

        public static async Task DependenciesManagerAsync(string fileUrl, string subDirectory)
        {
            try
            {
                string targetFolderPath = Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory);

                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                string fileExtension = Path.GetExtension(fileUrl).ToLower();

                if (fileExtension == ".zip")
                {
                    string tempZipFilePath = Path.Combine(AppConfig.PakMasterDependencyFolder, "temp_69420.zip");

                    using (HttpClient client = new())
                    {
                        byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);
                        await File.WriteAllBytesAsync(tempZipFilePath, fileBytes);
                    }

                    if (File.Exists(tempZipFilePath))
                    {
                        ZipFile.ExtractToDirectory(tempZipFilePath, targetFolderPath, true);
                        File.Delete(tempZipFilePath);
                        GLogger.Here().Information("Zip file extracted to: {TargetFolderPath}", targetFolderPath);
                    }
                    else
                    {
                        GLogger.Here().Error("Downloaded zip file not found.");
                    }
                }
                else
                {
                    string fileName = Path.GetFileName(fileUrl);
                    string targetFilePath = Path.Combine(targetFolderPath, fileName);

                    using (HttpClient client = new())
                    {
                        byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);
                        await File.WriteAllBytesAsync(targetFilePath, fileBytes);
                    }

                    GLogger.Here().Information("File downloaded to: {TargetFilePath}", targetFilePath);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An error occurred while downloading the file.");
            }
        }

        private static bool ShouldCheckForUpdates()
        {
            string timeFile = Path.Combine(AppConfig.PakMasterDependencyFolder, ".last_update_check");
            if (File.Exists(timeFile))
            {
                if (DateTime.TryParse(File.ReadAllText(timeFile), out DateTime lastCheck))
                {
                    if ((DateTime.UtcNow - lastCheck).TotalMinutes < 3)
                        return false;
                }
            }
            return true;
        }

        private static void MarkUpdateCheck()
        {
            string timeFile = Path.Combine(AppConfig.PakMasterDependencyFolder, ".last_update_check");
            Directory.CreateDirectory(AppConfig.PakMasterDependencyFolder);
            File.WriteAllText(timeFile, DateTime.UtcNow.ToString("o"));
        }

        private static async Task<string> GetLatestGitHubReleaseTagAsync(string repo)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "PakMaster-App");
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                string url = $"https://api.github.com/repos/{repo}/releases/latest";
                string json = await client.GetStringAsync(url);

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("tag_name", out var tagProp))
                {
                    return tagProp.GetString()?.Trim('v', ' ') ?? "";
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Warning(ex, $"Failed to check GitHub releases for {repo}.");
            }
            return "";
        }

        private static async Task<string> GetLocalToolVersionAsync(string subDirectory, string exeName)
        {
            try
            {
                string targetFilePath = Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory, exeName);
                if (!File.Exists(targetFilePath)) return "";

                using var process = new Process();
                process.StartInfo.FileName = targetFilePath;
                process.StartInfo.Arguments = "-V";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                var parts = output.Trim().Split(' ');
                if (parts.Length >= 2) return parts[1].Trim();
                return output.Trim();
            }
            catch (Exception ex)
            {
                GLogger.Here().Warning(ex, $"Failed to get local version for {exeName}.");
            }
            return "";
        }

        public static async Task CheckAndDownloadToolDependenciesAsync()
        {
            bool repakDownload = false;
            bool repakDownloaded = false;
            bool retocDownload = false;
            bool retocDownloaded = false;
            bool missingDependencies = false;

            bool checkUpdates = ShouldCheckForUpdates();

            // Check if repak.exe exists in bin/repak
            if (!CheckIfDependencyExists("repak", "repak.exe"))
            {
                bool userConfirmed = await MessageManager.ShowYesNo("Dependency Manager", "Repak is missing.\n\nWould you like to download it now?");

                if (userConfirmed)
                {
                    repakDownload = true;
                }
                else
                {
                    missingDependencies = true;
                }
            }
            else if (checkUpdates)
            {
                string localVer = await GetLocalToolVersionAsync("repak", "repak.exe");
                string latestVer = await GetLatestGitHubReleaseTagAsync("trumank/repak");
                if (!string.IsNullOrEmpty(localVer) && !string.IsNullOrEmpty(latestVer) && localVer != latestVer)
                {
                    bool userConfirmed = await MessageManager.ShowYesNo("Dependency Manager", $"An update for Repak is available ({localVer} -> {latestVer}).\n\nWould you like to update it now?");
                    if (userConfirmed) { repakDownload = true; }
                }
                else
                {
                    GLogger.Here().Debug("repak.exe is up to date.");
                }
            }
            else
            {
                GLogger.Here().Debug("repak.exe exists (update check skipped).");
            }

            // Check if retoc.exe exists in bin/retoc
            string retocExeName = CheckIfDependencyExists("retoc", "retoc.exe") ? "retoc.exe" : "Retoc.exe";

            if (!CheckIfDependencyExists("retoc", retocExeName))
            {
                bool userConfirmed = await MessageManager.ShowYesNo("Dependency Manager", "Retoc is missing.\n\nWould you like to download it now?");

                if (userConfirmed)
                {
                    retocDownload = true;
                }
                else
                {
                    missingDependencies = true;
                }
            }
            else if (checkUpdates)
            {
                string localVer = await GetLocalToolVersionAsync("retoc", retocExeName);
                string latestVer = await GetLatestGitHubReleaseTagAsync("trumank/retoc");
                if (!string.IsNullOrEmpty(localVer) && !string.IsNullOrEmpty(latestVer) && localVer != latestVer)
                {
                    bool userConfirmed = await MessageManager.ShowYesNo("Dependency Manager", $"An update for Retoc is available ({localVer} -> {latestVer}).\n\nWould you like to update it now?");
                    if (userConfirmed) { retocDownload = true; }
                }
                else
                {
                    GLogger.Here().Debug("Retoc.exe is up to date.");
                }
            }
            else
            {
                GLogger.Here().Debug("Retoc.exe exists (update check skipped).");
            }

            if (checkUpdates)
            {
                MarkUpdateCheck();
            }

            if (repakDownload)
            {
                await MessageManager.ShowProgress("Dependency Manager", "Downloading Repak\n\nPlease wait...", async progress =>
                {
                    var downloadAll = Task.Run(async () =>
                    {
                        await DependenciesManagerAsync(AppUrls.RepakUrl, "repak");

                        var extraTasks = new List<Task>();

                        if (!CheckIfDependencyExists("repak", "repak-chunked-compression.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakChunkedCompressionUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-cs-bindings.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakCSBindingsUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-back4blood.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakPatchBack4BloodUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-dead-by-daylight.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakDeadByDaylightUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-dragon-quest-xi.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakDragonQuestXiUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-marvel-rivals.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakMarvelRivalsUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-outlast-trials.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakOutlastTrialsUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-torchlight.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakTorchlightUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-visions-of-mana.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakVisionsOfManaUrl, "repak"));
                        if (!CheckIfDependencyExists("repak", "repak-patch-wuthering-waves.exe")) extraTasks.Add(DependenciesManagerAsync(AppUrls.RepakWutheringWavesUrl, "repak"));

                        if (extraTasks.Count > 0)
                        {
                            await Task.WhenAll(extraTasks);
                        }
                    });

                    for (int i = 0; i <= 100; i++)
                    {
                        if (downloadAll.IsCompleted) break;
                        await Task.Delay(50);
                        progress.Report(i / 100.0);
                    }

                    await downloadAll;
                    progress.Report(1.0);
                    repakDownloaded = true;
                });
            }

            if (retocDownload)
            {
                await MessageManager.ShowProgress("Dependency Manager", "Downloading Retoc\n\nPlease wait...", async progress =>
                {
                    var downloadDependency = DependenciesManagerAsync(AppUrls.RetocUrl, "retoc");

                    for (int i = 0; i <= 100; i++)
                    {
                        await Task.Delay(50);
                        progress.Report(i / 100.0);
                    }

                    await downloadDependency;
                    retocDownloaded = true;
                });
            }

            if (missingDependencies)
            {
                await MessageManager.ShowInfo("Dependency Manager", "Missing Dependencies!\n\nPakMaster will not work without the dependencies.");
                Application.Current.Shutdown();
            }
            else if (repakDownloaded || retocDownloaded)
            {
                await MessageManager.ShowInfo("Dependency Manager", "Dependency downloads complete!");
            }
        }
    }
}