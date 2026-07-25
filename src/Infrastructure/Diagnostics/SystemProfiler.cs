namespace PakMaster.Infrastructure.Diagnostics
{
    public static class SystemProfiler
    {
        public static string GenerateLogHeader(object initialSettings)
        {
            var appVersion = AppConfig.DisplayAppVersion;

            // Early load settings
            var settings = initialSettings as AppSettingsModel;

            var appLanguage = settings?.Language ?? "Unknown";
            var appTheme = settings?.AccentColor ?? "Unknown";
            var interfaceScale = settings?.InterfaceScale ?? 100;
            var startWithWindows = settings?.StartWithWindows ?? false;
            var alwaysOnTop = settings?.AlwaysOnTop ?? false;
            var minimizeToTray = settings?.MinimizeToTray ?? false;
            var checkForUpdatesOnStartup = settings?.CheckForUpdatesOnStartup ?? false;
            var windowLeft = settings?.WindowLeft ?? 0;
            var windowTop = settings?.WindowTop ?? 0;
            var windowWidth = settings?.WindowWidth ?? 0;
            var windowHeight = settings?.WindowHeight ?? 0;
            var windowIsMaximized = settings?.WindowIsMaximized ?? false;

            var activeMonitors = DisplayManager.GetActiveMonitors();
            int monitorCount = activeMonitors.Count;

            var displayLayoutSummary = "";
            foreach (var monitor in activeMonitors)
            {
                var primaryTag = monitor.IsPrimary ? " [Primary]" : "";

                var cleanName = monitor.DeviceName.Replace(@"\\.\", "");

                displayLayoutSummary += $"{Environment.NewLine}    {cleanName}: {monitor.Width}x{monitor.Height}{primaryTag}";
            }

            var buildConfig = "Release";

            return string.Join(Environment.NewLine,
                    "==================================================================",
                    $" {AppConfig.DisplayAppName}",
                    "==================================================================",
                    $"  Timestamp:               {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    $"  Time Zone Offset:        {DateTime.Now:zzz}",
                    $"  Version:                 v{appVersion}",
                    $"  Build Configuration:     {buildConfig}",
                    $"  Operating System:        {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})",
                    $"  .NET Runtime:            {RuntimeInformation.FrameworkDescription}",
                    $"  CPU Cores:               {GetPhysicalCoreCount()}",
                    $"  CPU Threads:             {Environment.ProcessorCount}",
                    $"  System Memory:           {GetTotalMemoryGB()} GB",
                    $"  Display Resolution:      {SystemParameters.PrimaryScreenWidth}x{SystemParameters.PrimaryScreenHeight}",
                    $"  Display Count:           {monitorCount}",
                    $"  Detected Displays:       {displayLayoutSummary}",
                    "------------------------------------------------------------------",
                    " USER SETTINGS:",
                    $"  Language:                {appLanguage}",
                    $"  Theme:                   {appTheme}",
                    $"  Interface Scale:         {interfaceScale}%",
                    $"  Application Behavior:    StartWithWindows: {startWithWindows} | AlwaysOnTop: {alwaysOnTop} | MinimizeToTray: {minimizeToTray}",
                    $"  Updates:                 CheckForUpdates: {checkForUpdatesOnStartup}",
                    $"  Window Position:         WindowLeft: {windowLeft} | WindowTop: {windowTop}",
                    $"  Window Resolution:       {windowWidth}x{windowHeight}",
                    $"  Window Maximized:        {windowIsMaximized}",
                    "=================================================================="
                );
        }

        private static string GetTotalMemoryGB()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var memStatus = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(memStatus))
                {
                    double totalRamBytes = memStatus.ullTotalPhys;
                    return Math.Round(totalRamBytes / (1024.0 * 1024.0 * 1024.0), 1).ToString();
                }
            }
            return "Unknown";
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        private const int LOGICAL_PROCESSOR_RELATION_CORE = 0;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetLogicalProcessorInformationEx(int relationshipType, IntPtr buffer, ref int returnedLength);

        public static int GetPhysicalCoreCount()
        {
            int length = 0;
            GetLogicalProcessorInformationEx(LOGICAL_PROCESSOR_RELATION_CORE, IntPtr.Zero, ref length);

            if (Marshal.GetLastWin32Error() != 122)
                return Environment.ProcessorCount;

            IntPtr buffer = Marshal.AllocHGlobal(length);
            try
            {
                if (GetLogicalProcessorInformationEx(LOGICAL_PROCESSOR_RELATION_CORE, buffer, ref length))
                {
                    int coreCount = 0;
                    int offset = 0;

                    while (offset < length)
                    {
                        int relationship = Marshal.ReadInt32(buffer, offset);
                        int size = Marshal.ReadInt32(buffer, offset + 4);

                        if (relationship == LOGICAL_PROCESSOR_RELATION_CORE)
                        {
                            coreCount++;
                        }

                        if (size == 0) break;
                        offset += size;
                    }

                    return coreCount > 0 ? coreCount : Environment.ProcessorCount;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

            return Environment.ProcessorCount;
        }
    }
}