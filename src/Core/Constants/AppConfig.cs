namespace PakMaster.Core.Constants
{
    public class AppConfig
    {
        // ============ Edit This Section ============

        // App License
        private static string _appLicense = "MIT"; // e.g., MIT, Apache, GPLv3, LGPLv3, etc.

        // DB Safe App Name
        public static string DbSafeAppName { get; } = "pak_master"; // e.g., pakmaster, vssuite, pak_master, etc.

        // ============ Only Edit Below If Necessary ============

        // ============ Application Info ============

        // App Name
        public static string AppName { get; } = Assembly.GetExecutingAssembly().GetName().Name ?? "Generic-WPF-Application";

        // Display App Name
        public static string DisplayAppName { get; } = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Generic WPF Application";

        // Display App Name Uppercase
        public static string DisplayAppNameUppercase { get; } = DisplayAppName.ToUpperInvariant();

        // App Author
        public static string AppAuthor { get; } = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown Author";

        // App Version
        public static string AppVersion => _assemblyVersion != null ? $"{_assemblyVersion.Major}.{_assemblyVersion.Minor}.{_assemblyVersion.Build}.{_assemblyVersion.Revision}" : "Unknown Version";

        // Display App Version
        public static string DisplayAppVersion => _assemblyVersion != null ? $"{_assemblyVersion.Major}.{_assemblyVersion.Minor}.{_assemblyVersion.Build}" : "Unknown Version";

        // OS Architecture
        public static string OsArchitecture { get; } = $"Windows {RuntimeInformation.ProcessArchitecture.ToString().Replace("X", "x")}";

        // Target Framework
        public static string TargetFramework { get; } = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName.Replace(".NETCoreApp,Version=v", ".NET ") ?? "Unknown .NET Version";

        // ============ File/Folder Paths ============

        // App Config Folder Path
        public static string AppConfigFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppAuthor, AppName);

        // App Settings File Path
        public static string AppSettingsPath { get; } = Path.Combine(AppConfigFolder, "AppSettings.json");

        // App Logs Folder Path
        public static string AppLogsFolder { get; } = Path.Combine(AppConfigFolder, "Logs");

        // App Dependency Folder Path
        public static string AppDependencyTrackingFolder { get; } = Path.Combine(AppConfigFolder, "Dependencies");

        // App Dependency File Path
        public static string AppDependencyTrackingPath { get; } = Path.Combine(AppDependencyTrackingFolder, "Installed_Dependencies.json");

        // ============ Helpers ============

        // App Version Helper
        private static readonly Version? _assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

        // App License Helper
        public static string AppLicense { get => string.IsNullOrWhiteSpace(_appLicense) ? "Unknown License" : _appLicense; set => _appLicense = value; }

        // ============ DO NOT EDIT ============

        // App Template Name
        public static string AppTemplateName { get; } = "PakMaster";

        // Display App Template Name
        public static string DisplayAppTemplateName { get; } = "Project Velocity";

        // App Template Author
        public static string AppTemplateAuthor { get; } = "AriesLR";

        // App Template Copyright
        public static string AppTemplateCopyright { get; } = "Copyright 2026 AriesLR";

        // App GUID
        public const string AppGuid = "$guid1$";

        // Other

        // This won't get you very far if you are a bad actor digging around.
        // Regardless of what you do with this information there are still several layers of security on the server side.
        // I guess if you want to add a fake user to my db every 15 days be my guest, it only makes me feel better about myself seeing number go up.
        // Also not worth trying to see the db, everything of even slight value is hashed.
        public static readonly byte[] WhyAreYouLookingHere = [104, 116, 116, 112, 115, 58, 47, 47, 97, 112, 105, 46, 97, 114, 105, 101, 115, 108, 114, 46, 120, 121, 122];

        public static readonly byte[] LookSomewhereElse = [99, 53, 105, 68, 114, 80, 50, 88, 48, 119, 118, 104, 113, 69];
    }
}