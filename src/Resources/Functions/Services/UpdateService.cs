using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;

namespace PakMaster.Resources.Functions.Services
{
    public static class UpdateService
    {
        private static readonly string currentVersionPakMaster = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
        public static async Task CheckJsonForUpdates(string jsonUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync(jsonUrl);
                    var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(response);

                    if (updateInfo == null)
                    {
                        MessageBox.Show("Failed to retrieve update information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var latestVersion = updateInfo.latestVersionPakMaster;
                    var currentVersion = currentVersionPakMaster;

                    int versionComparison = CompareVersions(currentVersion, latestVersion);

                    if (versionComparison < 0)
                    {
                        // New version available
                        var result = MessageBox.Show(
                            $"A new version is available: {latestVersion}\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nWould you like to download the new version?",
                            "Check for updates",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information
                        );

                        if (result == MessageBoxResult.Yes)
                        {
                            UrlService.OpenUrl(updateInfo.downloadUrlPakMaster);
                        }
                    }
                    else if (versionComparison > 0)
                    {
                        // Easter egg (this shouldn't happen, but I'm dumb)
                        MessageBox.Show(
                            $"You're a wizard, harry!\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nTell AriesLR he's a goofball and forgot to update the version number.",
                            "Check for updates",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    else
                    {
                        // Up to date
                        MessageBox.Show(
                            $"You are already using the latest version.\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}",
                            "Check for updates",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check for updates: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static int CompareVersions(string currentVersion, string latestVersion)
        {
            var currentParts = currentVersion.Split('.');
            var latestParts = latestVersion.Split('.');

            int maxLength = Math.Max(currentParts.Length, latestParts.Length);

            for (int i = 0; i < maxLength; i++)
            {
                int currentPart = i < currentParts.Length ? int.Parse(currentParts[i]) : 0;
                int latestPart = i < latestParts.Length ? int.Parse(latestParts[i]) : 0;

                if (currentPart < latestPart) return -1;
                if (currentPart > latestPart) return 1;
            }

            return 0;
        }


        public class UpdateInfo
        {
            public string latestVersionPakMaster { get; set; }
            public string downloadUrlPakMaster { get; set; }
        }
    }
}
