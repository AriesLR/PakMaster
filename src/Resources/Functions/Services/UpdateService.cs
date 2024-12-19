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
                    if (updateInfo.latestVersionPakMaster != currentVersionPakMaster)
                    {
                        // New version available, notify the user
                        var result = MessageBox.Show(
                            $"A new version is available: {updateInfo.latestVersionPakMaster}\n\nLatest Version: {updateInfo.latestVersionPakMaster}\nYour Version: {currentVersionPakMaster}\nWould you like to download the new version?",
                            "Check for updates",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information
                        );

                        // Check user's choice
                        if (result == MessageBoxResult.Yes)
                        {
                            UrlService.OpenUrl(updateInfo.downloadUrlPakMaster); // Use the UrlService to open the update link
                        }
                    }
                    else
                    {
                        MessageBox.Show($"You are already using the latest version.\n\nLatest Version: {updateInfo.latestVersionPakMaster}\nYour Version: {currentVersionPakMaster}", "Check for updates", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check for updates: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class UpdateInfo
        {
            public string latestVersionPakMaster { get; set; }
            public string downloadUrlPakMaster { get; set; }
        }
    }
}
