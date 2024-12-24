using PakMaster.Resources.Functions.Services;
using System.Diagnostics;
using System.Windows;

namespace PakMaster
{
    public partial class App : Application
    {
        private ConfigService _configService;

        private async void AppStartup(object sender, StartupEventArgs e)
        {
            _configService = new ConfigService();
            _configService.EnsureConfigsExist();

            Debug.WriteLine("[DEBUG]: Checking for updates in the background.");
            await Task.Delay(1500);
            await UpdateService.CheckJsonForUpdatesSilent("https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/version/update.json");

            bool repakDownload = false;
            bool repakDownloaded = false;
            bool zentoolsDownload = false;
            bool zentoolsDownloaded = false;
            bool missingDependencies = false;

            // Check if repak.exe exists in bin/repak
            if (!DependenciesService.CheckIfDependencyExists("repak", "repak.exe"))
            {
                var result = MessageBox.Show(
                    "Repak is missing.\n\nWould you like to download it now?",
                    "Dependency Manager",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
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
                var result = MessageBox.Show(
                    "ZenTools is missing.\n\nWould you like to download it now?",
                    "Dependency Manager",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
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

            if (repakDownload && zentoolsDownload)
            {
                await DependenciesService.DependenciesManagerAsync("https://github.com/trumank/repak/releases/download/v0.2.2/repak_cli-x86_64-pc-windows-msvc.zip", "repak");
                await Task.Delay(1000);
                repakDownloaded = true;
                await DependenciesService.DependenciesManagerAsync("https://github.com/LongerWarrior/ZenTools/releases/download/1.06UE5.1-5.2/ZenTools.exe", "zentools");
                await Task.Delay(1000);
                zentoolsDownloaded = true;
            }

            if (missingDependencies)
            {
                MessageBox.Show(
                    "Missing Dependencies!\n\nPakMaster will not work without the dependencies.",
                    "Dependency Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                Application.Current.Shutdown(); // Close the app
            }
            else if (repakDownloaded && zentoolsDownloaded)
            {
                MessageBox.Show(
                    "Dependency downloads complete!",
                    "Dependency Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}
