namespace PakMaster.Core.Models
{
    public class AppSettingsModel
    {
        // ================ Appearance ================

        // Language
        public string Language { get; set; } = "en";

        // Base Theme (Light/Dark)
        public string BaseTheme { get; set; } = "Dark"; // Light theme will work, but it won't look the best without changes to the current UI. I don't have plans to actually support light themes.

        // Accent Color (Amber/Blue/Brown/Cobalt/etc)
        public string AccentColor { get; set; } = "Lime";

        // Combined Theme Name
        public string CombinedThemeName => $"{BaseTheme}.{AccentColor}";

        // Interface Scale
        public int InterfaceScale { get; set; } = 100;

        // ================ Application Behavior ================
        public bool StartWithWindows { get; set; } = false;

        public bool AlwaysOnTop { get; set; } = false;

        public bool MinimizeToTray { get; set; } = false;

        // ================ Updates ================
        public bool CheckForUpdatesOnStartup { get; set; } = true;

        // ================ Logging ================
        public bool EnableDebugLogging { get; set; } = false;

        // ================ Window Settings ================
        public string DisplayName { get; set; } = "Primary";

        public double WindowLeft { get; set; } = 100;

        public double WindowTop { get; set; } = 100;

        public double WindowWidth { get; set; } = 1200;

        public double WindowHeight { get; set; } = 750;

        public bool WindowIsMaximized { get; set; } = false;

        public AppSettingsModel()
        {
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            WindowLeft = (screenWidth - WindowWidth) / 2;
            WindowTop = (screenHeight - WindowHeight) / 2;
        }
    }
}