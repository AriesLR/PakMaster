namespace PakMaster.Infrastructure.Display
{
    public static class DisplayManager
    {
        // Compiler will cry about using DllImport, it's fine, it's more effort than it's worth rewriting this for the severity level.
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(nint hMonitor, nint hdcMonitor, MonitorEnumProc lpfnEnum, nint dwData);

        private delegate bool MonitorEnumProc(nint hMonitor, nint hdcMonitor, ref Rect lprcMonitor, nint dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);

        [DllImport("user32.dll")]
        private static extern nint MonitorFromWindow(nint hwnd, uint dwFlags);

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const uint MONITORINFOF_PRIMARY = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfoEx
        {
            public int Size;
            public Rect MonitorRect;
            public Rect WorkRect;
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }

        public static List<MonitorDetailsModel> GetActiveMonitors()
        {
            var monitors = new List<MonitorDetailsModel>();

            EnumDisplayMonitors(nint.Zero, nint.Zero, Callback, nint.Zero);

            bool Callback(nint hMonitor, nint hdcMonitor, ref Rect lprcMonitor, nint dwData)
            {
                var info = new MonitorInfoEx
                {
                    Size = Marshal.SizeOf(typeof(MonitorInfoEx))
                };

                if (GetMonitorInfo(hMonitor, ref info))
                {
                    var details = new MonitorDetailsModel
                    {
                        DeviceName = info.DeviceName,
                        IsPrimary = (info.Flags & MONITORINFOF_PRIMARY) != 0,
                        Left = info.MonitorRect.Left,
                        Top = info.MonitorRect.Top,
                        Right = info.MonitorRect.Right,
                        Bottom = info.MonitorRect.Bottom
                    };

                    GLogger.Here().Debug("Enumerated display: Device={DeviceName}, Primary={IsPrimary}, Bounds=({Left},{Top}) to ({Right},{Bottom})",
                        details.DeviceName, details.IsPrimary, details.Left, details.Top, details.Right, details.Bottom);

                    monitors.Add(details);
                }
                else
                {
                    GLogger.Here().Warning("Failed to retrieve display structural information for handle: {MonitorHandle}", hMonitor);
                }
                return true;
            }

            GLogger.Here().Information("Successfully enumerated {Count} active display(s).", monitors.Count);
            return monitors;
        }

        // Get Monitor From Window
        public static MonitorDetailsModel? GetMonitorFromWindow(Window window)
        {
            var helper = new WindowInteropHelper(window);
            nint hwnd = helper.Handle;

            if (hwnd == nint.Zero)
            {
                GLogger.Here().Debug("Unable to resolve active handle for window.");
                return null;
            }

            nint hMonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            var info = new MonitorInfoEx
            {
                Size = Marshal.SizeOf(typeof(MonitorInfoEx))
            };

            if (GetMonitorInfo(hMonitor, ref info))
            {
                var details = new MonitorDetailsModel
                {
                    DeviceName = info.DeviceName,
                    IsPrimary = (info.Flags & MONITORINFOF_PRIMARY) != 0,
                    Left = info.MonitorRect.Left,
                    Top = info.MonitorRect.Top,
                    Right = info.MonitorRect.Right,
                    Bottom = info.MonitorRect.Bottom
                };

                GLogger.Here().Debug("Resolved window placement: Window={WindowType}, TargetDevice={DeviceName}, IsPrimary={IsPrimary}", window.GetType().Name, details.DeviceName, details.IsPrimary);

                return details;
            }

            GLogger.Here().Warning("Could not find which monitor contains window handle: {Hwnd}", hwnd);
            return null;
        }
    }
}