namespace PakMaster.UI.Taskbar
{
    public static class TrayIconManager
    {
        public static void SetVisibility(bool isVisible)
        {
            var appIcon = GetTaskbarIcon();

            if (appIcon != null)
            {
                appIcon.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                GLogger.Here().Information("Successfully updated TaskbarIcon visibility to: {Visibility}", appIcon.Visibility);
            }
            else
            {
                GLogger.Here().Warning("Could not resolve TaskbarIcon instance anywhere within the MainWindow.");
            }
        }

        public static void UpdateTrayIconVisibility()
        {
            if (AppSettingsManager.CurrentSettings == null) return;

            var appIcon = GetTaskbarIcon();

            if (appIcon != null)
            {
                bool isOn = AppSettingsManager.CurrentSettings.MinimizeToTray;
                appIcon.Visibility = isOn ? Visibility.Visible : Visibility.Collapsed;
                GLogger.Here().Information("Set initial TaskbarIcon visibility to: {Visibility}", appIcon.Visibility);
            }
            else
            {
                GLogger.Here().Warning("Could not resolve TaskbarIcon instance anywhere within the MainWindow.");
            }
        }

        private static TaskbarIcon? GetTaskbarIcon()
        {
            MainWindow? mainWindow = null;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow targetWindow)
                {
                    mainWindow = targetWindow;
                    break;
                }
            }

            if (mainWindow == null)
                return null;

            return FindLogicalChild<TaskbarIcon>(mainWindow);
        }

        public static T? FindLogicalChild<T>(DependencyObject? parent) where T : DependencyObject
        {
            if (parent == null) return null;

            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is T target) return target;

                if (child is DependencyObject depObj)
                {
                    var result = FindLogicalChild<T>(depObj);
                    if (result != null) return result;
                }
            }
            return null;
        }
    }
}