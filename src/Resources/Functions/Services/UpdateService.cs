using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace PakMaster.Resources.Functions.Services
{
    public class UpdateService
    {
        private static readonly string currentVersionPakMaster = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
        public static async Task CheckJsonForUpdatesAsync(string jsonUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync(jsonUrl);
                    var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(response);

                    if (updateInfo == null)
                    {
                        await MessageService.ShowError("Failed to retrieve update information.");
                        return;
                    }

                    var latestVersion = updateInfo.latestVersionPakMaster;
                    var currentVersion = currentVersionPakMaster;

                    int versionComparison = CompareVersions(currentVersion, latestVersion);

                    if (versionComparison < 0)
                    {
                        // New version available
                        bool userConfirmed = await MessageService.ShowYesNo("Check For Updates", $"A new version is available: {latestVersion}\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nWould you like to download the new version?");

                        if (userConfirmed)
                        {
                            UrlService.OpenUrlAsync(updateInfo.downloadUrlPakMaster);
                        }
                    }
                    else if (versionComparison > 0)
                    {
                        // Easter egg (this shouldn't happen, but I'm dumb)
                        await MessageService.ShowInfo("Check For Updates", $"You're a wizard, harry!\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nTell AriesLR he's a goofball and forgot to update the version number.");
                    }
                    else
                    {
                        // Up to date
                        await MessageService.ShowInfo("Check For Updates", $"You are already using the latest version.\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}");
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Failed to check for updates: {ex.Message}");
            }
        }
        public static async Task CheckJsonForUpdatesAsyncSilent(string jsonUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync(jsonUrl);
                    var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(response);

                    if (updateInfo == null)
                    {
                        return;
                    }

                    var latestVersion = updateInfo.latestVersionPakMaster;
                    var currentVersion = currentVersionPakMaster;

                    int versionComparison = CompareVersions(currentVersion, latestVersion);

                    if (versionComparison < 0)
                    {
                        bool userConfirmed = await MessageService.ShowYesNo("Check For Updates", $"A new version is available: {latestVersion}\n\nLatest Version: {latestVersion}\nYour Version: {currentVersion}\n\nWould you like to download the new version?");

                        if (userConfirmed)
                        {
                            UrlService.OpenUrlAsync(updateInfo.downloadUrlPakMaster);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to check for updates: {ex.Message}");
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
