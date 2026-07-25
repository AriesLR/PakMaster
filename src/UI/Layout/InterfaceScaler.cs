namespace PakMaster.UI.Layout
{
    public static class InterfaceScaler
    {
        // Apply Interface Scale
        public static void ApplyInterfaceScale(int scalePercentage)
        {
            Application.Current.Dispatcher.Invoke(() =>
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
                {
                    GLogger.Here().Warning("Could not locate a valid active instance of MainWindow.");
                    return;
                }

                if (!mainWindow.IsInitialized)
                {
                    return;
                }

                if (mainWindow.MainLayoutScale == null)
                {
                    GLogger.Here().Warning("'MainLayoutScale' reference is null inside MainWindow.");
                    return;
                }

                if (mainWindow.FlyoutsContainer == null)
                {
                    GLogger.Here().Warning("'FlyoutsContainer' reference is null inside MainWindow.");
                    return;
                }

                double scaleFactor = scalePercentage / 100.0;

                mainWindow.MainLayoutScale.ScaleX = scaleFactor;
                mainWindow.MainLayoutScale.ScaleY = scaleFactor;

                if (mainWindow.FlyoutsContainer.LayoutTransform is ScaleTransform flyoutScale)
                {
                    flyoutScale.ScaleX = scaleFactor;
                    flyoutScale.ScaleY = scaleFactor;
                }
                else
                {
                    mainWindow.FlyoutsContainer.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);
                }

                GLogger.Here().Debug("Global UI layout scale factor applied: {Factor} ({Percent}%)", scaleFactor, scalePercentage);
            });
        }

        // Execute Scale Update
        public static async Task ExecuteScaleUpdate(int targetScale, bool saveAndNotify = false)
        {
            ApplyInterfaceScale(targetScale);

            if (!saveAndNotify) return;

            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null)
                return;

            AppSettingsManager.CurrentSettings.InterfaceScale = targetScale;
            AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            await App.Toasts.ShowConfigSaved();
        }
    }
}