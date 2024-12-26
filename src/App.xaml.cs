using PakMaster.Resources.Functions.Services;
using System.Diagnostics;
using System.Windows;

namespace PakMaster
{
    public partial class App : Application
    {
        private ConfigService _configService;

        private async void AppStartupAsync(object sender, StartupEventArgs e)
        {
            _configService = new ConfigService();
            _configService.EnsureConfigsExist();

            Debug.WriteLine("[DEBUG]: Checking for updates in the background.");
            await Task.Delay(1500);
            await UpdateService.CheckJsonForUpdatesAsyncSilent("https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/version/update.json");

            bool repakDownload = false;
            bool repakDownloaded = false;
            bool zentoolsDownload = false;
            bool zentoolsDownloaded = false;
            bool missingDependencies = false;

            // Check if repak.exe exists in bin/repak
            if (!DependenciesService.CheckIfDependencyExists("repak", "repak.exe"))
            {
                bool userConfirmed = await MessageService.ShowYesNo("Dependency Manager", "Repak is missing.\n\nWould you like to download it now?");

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
                Debug.WriteLine("[DEBUG]: repak.exe already exists.");
            }

            // Check if zentools.exe exists in bin/zentools
            if (!DependenciesService.CheckIfDependencyExists("zentools", "ZenTools.exe"))
            {
                bool userConfirmed = await MessageService.ShowYesNo("Dependency Manager", "ZenTools is missing.\n\nWould you like to download it now?");

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
                Debug.WriteLine("[DEBUG]: ZenTools.exe already exists.");
            }

            if (repakDownload && zentoolsDownload) // Loading bars are fake, maybe I'll add actual download tracking in the future.
            {
                await MessageService.ShowProgress("Dependency Manager", "Downloading Repak\n\nPlease wait...", async progress =>
                {
                    var downloadDependency = DependenciesService.DependenciesManagerAsync("https://github.com/trumank/repak/releases/download/v0.2.2/repak_cli-x86_64-pc-windows-msvc.zip", "repak");

                    for (int i = 0; i <= 100; i++)
                    {
                        await Task.Delay(50);
                        progress.Report(i / 100.0);
                    }

                    await downloadDependency;

                    repakDownloaded = true;
                });

                await MessageService.ShowProgress("Dependency Manager", "Downloading ZenTools\n\nPlease wait...", async progress =>
                {
                    var downloadDependency = DependenciesService.DependenciesManagerAsync("https://github.com/LongerWarrior/ZenTools/releases/download/1.06UE5.1-5.2/ZenTools.exe", "zentools");

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
                await MessageService.ShowInfo("Dependency Manager", "Missing Dependencies!\n\nPakMaster will not work without the dependencies.");
                Application.Current.Shutdown(); // Close the app
            }
            else if (repakDownloaded && zentoolsDownloaded)
            {
                await MessageService.ShowInfo("Dependency Manager", "Dependency downloads complete!");
            }
        }
    }
}
