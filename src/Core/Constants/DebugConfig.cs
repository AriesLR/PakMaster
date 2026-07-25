namespace PakMaster.Core.Constants
{
    public class DebugConfig
    {
        public static bool DebugSplashScreen { get; } = false; // Set to True to enable titlebar for Splash Screen
        public static bool LockSplashScreen { get; } = false; // Set to True to prevent the Splash Screen from progressing to the main window
        public static bool Telemetry { get; } = true; // Set to False to disable the TelemetryService; turn this off while in dev so you don't spam the api.
        public static bool Dependency { get; } = true; // Set to False to disable the DependencyService
    }
}