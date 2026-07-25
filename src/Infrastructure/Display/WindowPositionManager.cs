namespace PakMaster.Infrastructure.Display
{
    public static class WindowPositionManager
    {
        public static void RestorePosition(Window window)
        {
            if (AppSettingsManager.CurrentSettings == null) return;

            var settings = AppSettingsManager.CurrentSettings;
            var activeMonitors = DisplayManager.GetActiveMonitors();

            var targetMonitor = activeMonitors.FirstOrDefault(m => m.DeviceName == settings.DisplayName);

            if (targetMonitor == null || settings.DisplayName == "Primary")
            {
                targetMonitor = activeMonitors.FirstOrDefault(m => m.IsPrimary) ?? activeMonitors.FirstOrDefault();
                GLogger.Here().Warning("Target monitor '{SavedMonitor}' unavailable. Falling back to primary display.", settings.DisplayName);
            }

            if (targetMonitor != null)
            {
                double width = Math.Min(settings.WindowWidth, targetMonitor.Width);
                double height = Math.Min(settings.WindowHeight, targetMonitor.Height);

                double left = targetMonitor.Left + settings.WindowLeft;
                double top = targetMonitor.Top + settings.WindowTop;

                if (left < targetMonitor.Left || left + width > targetMonitor.Right ||
                    top < targetMonitor.Top || top + height > targetMonitor.Bottom)
                {
                    window.Left = targetMonitor.Left + (targetMonitor.Width - width) / 2;
                    window.Top = targetMonitor.Top + (targetMonitor.Height - height) / 2;
                    window.Width = width;
                    window.Height = height;
                }
                else
                {
                    window.Width = width;
                    window.Height = height;
                    window.Left = left;
                    window.Top = top;
                }

                if (settings.WindowIsMaximized)
                {
                    window.WindowState = WindowState.Maximized;
                }
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        // Save Window Position
        public static void SavePosition(Window window)
        {
            if (AppSettingsManager.CurrentSettings == null) return;

            var settings = AppSettingsManager.CurrentSettings;
            var activeMonitors = DisplayManager.GetActiveMonitors();

            double currentLeft = window.WindowState == WindowState.Maximized ? window.RestoreBounds.Left : window.Left;
            double currentTop = window.WindowState == WindowState.Maximized ? window.RestoreBounds.Top : window.Top;
            double currentWidth = window.WindowState == WindowState.Maximized ? window.RestoreBounds.Width : window.Width;
            double currentHeight = window.WindowState == WindowState.Maximized ? window.RestoreBounds.Height : window.Height;

            MonitorDetailsModel? hostingMonitor = null;
            double largestOverlapArea = 0;

            foreach (var monitor in activeMonitors)
            {
                double intersectLeft = Math.Max(currentLeft, monitor.Left);
                double intersectTop = Math.Max(currentTop, monitor.Top);
                double intersectRight = Math.Min(currentLeft + currentWidth, monitor.Right);
                double intersectBottom = Math.Min(currentTop + currentHeight, monitor.Bottom);

                if (intersectRight > intersectLeft && intersectBottom > intersectTop)
                {
                    double overlapArea = (intersectRight - intersectLeft) * (intersectBottom - intersectTop);

                    if (overlapArea > largestOverlapArea)
                    {
                        largestOverlapArea = overlapArea;
                        hostingMonitor = monitor;
                    }
                }
            }

            hostingMonitor ??= activeMonitors.FirstOrDefault(m => m.IsPrimary) ?? activeMonitors.FirstOrDefault();

            if (hostingMonitor != null)
            {
                settings.DisplayName = hostingMonitor.DeviceName;

                settings.WindowLeft = currentLeft - hostingMonitor.Left;
                settings.WindowTop = currentTop - hostingMonitor.Top;
                settings.WindowWidth = currentWidth;
                settings.WindowHeight = currentHeight;
                settings.WindowIsMaximized = window.WindowState == WindowState.Maximized;

                AppSettingsManager.SaveAppSettings(settings);
            }
        }

        // Reset Window Position
        public static void ResetWindowPosition()
        {
            if (AppSettingsManager.CurrentSettings == null) return;

            var settings = AppSettingsManager.CurrentSettings;
            var activeMonitors = DisplayManager.GetActiveMonitors();
            var primaryMonitor = activeMonitors.FirstOrDefault(m => m.IsPrimary) ?? activeMonitors.FirstOrDefault();

            if (primaryMonitor != null)
            {
                settings.DisplayName = primaryMonitor.DeviceName;
                settings.WindowWidth = 1200;
                settings.WindowHeight = 750;

                settings.WindowLeft = (primaryMonitor.Width - settings.WindowWidth) / 2;
                settings.WindowTop = (primaryMonitor.Height - settings.WindowHeight) / 2;
                settings.WindowIsMaximized = false;

                Window window = Application.Current.MainWindow;
                if (window != null)
                {
                    window.WindowState = WindowState.Normal;
                    window.Width = settings.WindowWidth;
                    window.Height = settings.WindowHeight;
                    window.Left = primaryMonitor.Left + settings.WindowLeft;
                    window.Top = primaryMonitor.Top + settings.WindowTop;
                }

                AppSettingsManager.SaveAppSettings(settings);
                GLogger.Here().Information("Window positions reset cleanly to Primary Display's center coordinates.");
            }
        }
    }
}