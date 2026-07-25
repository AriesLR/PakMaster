namespace PakMaster.Infrastructure.Web
{
    public static class UrlOperations
    {
        public static async void OpenUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                GLogger.Here().Warning("The provided target destination URL was null or empty.");
                return;
            }

            GLogger.Here().Information("Attempting to open target destination in browser. URL: {TargetUrl}", url);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to open URL: {TargetUrl}", url);

                string messageDesc = string.Format(Lang.UrlService_Msg_FailedToOpenURL_Desc, ex.Message);

                await MessageManager.ShowError(messageDesc);
            }
        }
    }
}