namespace PakMaster.Infrastructure.Web
{
    public static class UpdateManager
    {
        private static readonly HttpClient _httpClient = new();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task InitializeAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (AppSettingsManager.CurrentSettings.CheckForUpdatesOnStartup)
            {
                try
                {
                    GLogger.Here().Debug("Triggering silent update check on startup sequence.");
                    stopwatch.Stop();
                    await CheckForUpdatesSilentAsync(AppUrls.UpdateUrl);
                    stopwatch.Start();
                }
                catch (Exception ex)
                {
                    GLogger.Here().Error(ex, "An unexpected error occurred during the startup background update check.");
                }
            }
            else
            {
                GLogger.Here().Debug("Startup update check is disabled by user configuration profile.");
            }

            stopwatch.Stop();

            GLogger.Here().Information("Initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
        }

        // ============ Main Methods ============
        public static async Task CheckForUpdatesAsync(string jsonUrl)
        {
            GLogger.Here().Information("Initiating manual update check against endpoint: {UpdateUrl}", jsonUrl);

            try
            {
                string response = await _httpClient.GetStringAsync(jsonUrl);

                GLogger.Here().Debug("Successfully fetched update response from server.");
                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(response, _jsonOptions);

                if (updateInfo?.LatestVersion == null || updateInfo.DownloadUrl == null)
                {
                    GLogger.Here().Warning("Update check failed: Received schema was invalid or incomplete. Raw JSON: {RawJson}", response);
                    await MessageManager.ShowError(Lang.UpdateService_Msg_FailedToRetrieve_Desc);
                    return;
                }

                string latestVersion = updateInfo.LatestVersion;
                string currentVersion = AppConfig.AppVersion;

                int versionComparison = CompareVersions(currentVersion, latestVersion);
                GLogger.Here().Information("Version comparison complete. Local: {LocalVersion} | Remote: {RemoteVersion}", currentVersion, latestVersion);

                if (versionComparison < 0)
                {
                    GLogger.Here().Information("A newer version is available. Prompting user for download authorization.");

                    string messageDesc = string.Format(Lang.UpdateService_Msg_NewVersion_Desc, latestVersion, currentVersion);

                    bool userConfirmed = await MessageManager.ShowYesNo(Lang.UpdateService_Msg_CheckForUpdates_Title, messageDesc);

                    if (userConfirmed)
                    {
                        GLogger.Here().Information("User accepted update redirection. Requesting browser routing to: {DownloadUrl}", updateInfo.DownloadUrl);
                        UrlOperations.OpenUrlAsync(updateInfo.DownloadUrl);
                    }
                    else
                    {
                        GLogger.Here().Debug("User dismissed update prompt download aborted.");
                    }
                }
                else if (versionComparison > 0)
                {
                    string messageDesc = string.Format(Lang.UpdateService_Msg_EasterEgg_Desc, latestVersion, currentVersion);

                    await MessageManager.ShowInfo(Lang.UpdateService_Msg_CheckForUpdates_Title, messageDesc);
                }
                else
                {
                    string messageDesc = string.Format(Lang.UpdateService_Msg_UpToDate_Desc, latestVersion, currentVersion);

                    await MessageManager.ShowInfo(Lang.UpdateService_Msg_CheckForUpdates_Title, messageDesc);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An unhandled exception occurred during update process targeting: {UpdateUrl}", jsonUrl);

                string messageDesc = string.Format(Lang.UpdateService_Msg_FailedToCheck_Desc, ex.Message);

                await MessageManager.ShowError(messageDesc);
            }
        }

        public static async Task CheckForUpdatesSilentAsync(string jsonUrl)
        {
            try
            {
                string response = await _httpClient.GetStringAsync(jsonUrl);
                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(response, _jsonOptions);

                if (updateInfo?.LatestVersion == null || updateInfo.DownloadUrl == null)
                {
                    GLogger.Here().Warning("Silent update check failed: Malformed payload returned from endpoint.");
                    return;
                }

                string latestVersion = updateInfo.LatestVersion;
                string currentVersion = AppConfig.AppVersion;

                int versionComparison = CompareVersions(currentVersion, latestVersion);

                if (versionComparison < 0)
                {
                    GLogger.Here().Information("A newer version is available. Prompting user for download authorization.");
                    string messageDesc = string.Format(Lang.UpdateService_Msg_NewVersion_Desc, latestVersion, currentVersion);

                    bool userConfirmed = await MessageManager.ShowYesNo(Lang.UpdateService_Msg_CheckForUpdates_Title, messageDesc);

                    if (userConfirmed)
                    {
                        GLogger.Here().Information("User accepted update redirection. Requesting browser routing to: {DownloadUrl}", updateInfo.DownloadUrl);
                        UrlOperations.OpenUrlAsync(updateInfo.DownloadUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An unhandled exception occurred during silent update process targeting: {UpdateUrl}", jsonUrl);
            }
        }

        private static int CompareVersions(string currentVersion, string latestVersion)
        {
            try
            {
                if (NuGet.Versioning.NuGetVersion.TryParse(currentVersion, out var currentNugetVer) &&
                    NuGet.Versioning.NuGetVersion.TryParse(latestVersion, out var latestNugetVer))
                {
                    return currentNugetVer.CompareTo(latestNugetVer);
                }

                GLogger.Here().Warning("Failed to parse versions as NuGetVersion. Local='{Local}' Remote='{Remote}'", currentVersion, latestVersion);
                return 0;
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to compare versions. Strings: Local='{Local}' Remote='{Remote}'", currentVersion, latestVersion);
                return 0;
            }
        }

        public class UpdateInfo
        {
            public string? LatestVersion { get; set; }
            public string? DownloadUrl { get; set; }
        }
    }
}