using System.IO.Compression;

namespace PakMaster.Core.Managers
{
    public static class ToolDependencyManager
    {
        // Method to check if dependencies exist in the specified subdirectory
        public static bool CheckIfDependencyExists(string subDirectory, string exeName)
        {
            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", subDirectory);
            string targetFilePath = Path.Combine(targetDirectory, exeName);

            return File.Exists(targetFilePath);
        }

        public static async Task DependenciesManagerAsync(string fileUrl, string subDirectoryName)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string targetFolderPath = Path.Combine(baseDirectory, "bin", subDirectoryName);

                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                string fileExtension = Path.GetExtension(fileUrl).ToLower();

                if (fileExtension == ".zip")
                {
                    string tempZipFilePath = Path.Combine(baseDirectory, "temp_69420.zip");

                    using (HttpClient client = new HttpClient())
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

                    using (HttpClient client = new HttpClient())
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

        public static async Task CheckAndDownloadToolDependenciesAsync()
        {
            bool repakDownload = false;
            bool repakDownloaded = false;
            bool zentoolsDownload = false;
            bool zentoolsDownloaded = false;
            bool missingDependencies = false;

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
            else
            {
                GLogger.Here().Debug("repak.exe already exists.");
            }

            // Check if zentools.exe exists in bin/zentools
            if (!CheckIfDependencyExists("zentools", "ZenTools.exe"))
            {
                bool userConfirmed = await MessageManager.ShowYesNo("Dependency Manager", "ZenTools is missing.\n\nWould you like to download it now?");

                if (userConfirmed)
                {
                    zentoolsDownload = true;
                }
                else
                {
                    missingDependencies = true;
                }
            }
            else
            {
                GLogger.Here().Debug("ZenTools.exe already exists.");
            }

            if (repakDownload && zentoolsDownload)
            {
                await MessageManager.ShowProgress("Dependency Manager", "Downloading Repak\n\nPlease wait...", async progress =>
                {
                    var downloadDependency = DependenciesManagerAsync(AppUrls.RepakUrl, "repak");

                    for (int i = 0; i <= 100; i++)
                    {
                        await Task.Delay(50);
                        progress.Report(i / 100.0);
                    }

                    await downloadDependency;
                    repakDownloaded = true;
                });

                await MessageManager.ShowProgress("Dependency Manager", "Downloading ZenTools\n\nPlease wait...", async progress =>
                {
                    var downloadDependency = DependenciesManagerAsync(AppUrls.ZenToolsUrl, "zentools");

                    for (int i = 0; i <= 100; i++)
                    {
                        await Task.Delay(50);
                        progress.Report(i / 100.0);
                    }

                    await downloadDependency;
                    zentoolsDownloaded = true;
                });
            }

            if (missingDependencies)
            {
                await MessageManager.ShowInfo("Dependency Manager", "Missing Dependencies!\n\nPakMaster will not work without the dependencies.");
                Application.Current.Shutdown();
            }
            else if (repakDownloaded && zentoolsDownloaded)
            {
                await MessageManager.ShowInfo("Dependency Manager", "Dependency downloads complete!");
            }
        }
    }
}