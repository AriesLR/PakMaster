namespace PakMaster.Resources.Functions.Services
{
    public static class UrlService
    {
        public static async void OpenUrlAsync(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Failed to open URL: {ex.Message}");
            }
        }
    }
}
