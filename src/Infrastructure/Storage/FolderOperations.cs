namespace PakMaster.Infrastructure.Storage
{
    public static class FolderOperations
    {
        public static async Task OpenFolderAsync(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                GLogger.Here().Warning("Target folder path was null or empty.");
                return;
            }

            GLogger.Here().Information("Attempting to open folder at: {TargetFolder}", folderPath);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    GLogger.Here().Information("Target directory does not exist on disk: {TargetFolder}", folderPath);

                    string messageDesc = string.Format(Lang.FolderService_Msg_DoesNotExist_Desc, folderPath);

                    await MessageManager.ShowWarning(messageDesc);
                    return;
                }

                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = folderPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to open folder in File Explorer. Path: {TargetFolder}", folderPath);

                string messageDesc = string.Format(Lang.FolderService_Msg_FailedToOpen_Desc, ex.Message);

                await MessageManager.ShowError(messageDesc);
            }
        }
    }
}