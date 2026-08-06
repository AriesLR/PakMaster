namespace PakMaster.Core.Engines
{
    public static class ToolDependencyEngine
    {
        public static event Action? PackagesUpdated;

        public static List<PackageModel> AvailablePackages = new()
        {
            new PackageModel { DisplayName = "Retoc (Main)", ExecutableName = "retoc.exe", DownloadUrl = AppUrls.RetocUrl, ToolType = "Retoc" },
            new PackageModel { DisplayName = "Repak (Main)", ExecutableName = "repak.exe", DownloadUrl = AppUrls.RepakUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Chunked Compression", ExecutableName = "repak-chunked-compression.exe", DownloadUrl = AppUrls.RepakChunkedCompressionUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - CS Bindings", ExecutableName = "repak-cs-bindings.exe", DownloadUrl = AppUrls.RepakCSBindingsUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Back4Blood", ExecutableName = "repak-patch-back4blood.exe", DownloadUrl = AppUrls.RepakPatchBack4BloodUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Dead by Daylight", ExecutableName = "repak-patch-dead-by-daylight.exe", DownloadUrl = AppUrls.RepakDeadByDaylightUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Dragon Quest XI", ExecutableName = "repak-patch-dragon-quest-xi.exe", DownloadUrl = AppUrls.RepakDragonQuestXiUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Marvel Rivals", ExecutableName = "repak-patch-marvel-rivals.exe", DownloadUrl = AppUrls.RepakMarvelRivalsUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Outlast Trials", ExecutableName = "repak-patch-outlast-trials.exe", DownloadUrl = AppUrls.RepakOutlastTrialsUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Torchlight", ExecutableName = "repak-patch-torchlight.exe", DownloadUrl = AppUrls.RepakTorchlightUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Visions of Mana", ExecutableName = "repak-patch-visions-of-mana.exe", DownloadUrl = AppUrls.RepakVisionsOfManaUrl, ToolType = "Repak" },
            new PackageModel { DisplayName = "Repak - Wuthering Waves", ExecutableName = "repak-patch-wuthering-waves.exe", DownloadUrl = AppUrls.RepakWutheringWavesUrl, ToolType = "Repak" },
        };

        public static bool CheckIfDependencyExists(string subDirectory, string exeName)
        {
            if (exeName.Equals("retoc.exe", StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory, "retoc.exe")))
                    return true;
                if (File.Exists(Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory, "Retoc.exe")))
                    return true;
                return false;
            }

            string targetDirectory = Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory);
            string targetFilePath = Path.Combine(targetDirectory, exeName);

            return File.Exists(targetFilePath);
        }

        public static void UpdatePackageStates()
        {
            foreach (var pkg in AvailablePackages)
            {
                pkg.IsInstalled = CheckIfDependencyExists(pkg.ToolType.ToLower(), pkg.ExecutableName);
            }
        }

        public static List<PackageModel> GetAvailableBranches(string toolType)
        {
            UpdatePackageStates();
            return AvailablePackages.Where(p => p.ToolType.Equals(toolType, StringComparison.OrdinalIgnoreCase) && p.IsInstalled).ToList();
        }

        public static List<PackageModel> GetAllPackages()
        {
            UpdatePackageStates();
            return AvailablePackages;
        }

        public static bool UninstallDependency(string subDirectory, string exeName)
        {
            try
            {
                string targetPath = Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory, exeName);
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                else if (exeName.Equals("retoc.exe", StringComparison.OrdinalIgnoreCase))
                {
                    string alternatePath = Path.Combine(AppConfig.PakMasterDependencyFolder, subDirectory, "Retoc.exe");
                    if (File.Exists(alternatePath))
                    {
                        File.Delete(alternatePath);
                    }
                }
                
                UpdatePackageStates();
                PackagesUpdated?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, $"Failed to uninstall dependency {exeName}.");
                return false;
            }
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

                UpdatePackageStates();
                PackagesUpdated?.Invoke();
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An error occurred while downloading the file.");
            }
        }
    }
}