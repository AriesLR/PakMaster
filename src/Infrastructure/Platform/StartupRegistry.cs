namespace PakMaster.Infrastructure.Platform
{
    public static class StartupRegistry
    {
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void SetStartWithWindows(string appName, bool enable)
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);

            if (key == null)
            {
                GLogger.Here().Warning("Registry.CurrentUser.OpenSubKey returned null for the startup path.");
                throw new InvalidOperationException("Unable to open the Windows Registry startup key.");
            }

            if (enable)
            {
                string executablePath = Environment.ProcessPath ?? string.Empty;

                if (string.IsNullOrEmpty(executablePath))
                {
                    GLogger.Here().Warning("Failed to resolve the executable's file path.");
                    throw new InvalidOperationException("Failed to resolve application process path.");
                }

                key.SetValue(appName, $"\"{executablePath}\"");
                GLogger.Here().Debug("Successfully added the application to startup programs.");
            }
            else
            {
                if (key.GetValue(appName) != null)
                {
                    key.DeleteValue(appName);
                    GLogger.Here().Debug("Successfully removed the application from startup programs.");
                }
            }
        }
    }
}