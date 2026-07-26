namespace PakMaster.Core.Constants
{
    public static class AppUrls
    {
        // ============ Edit This Section ============

        // Update Tracking URL
        public const string UpdateUrl = "https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/version/update-new.json";

        // App License URL
        private static string _appLicenseUrl = "https://github.com/AriesLR/PakMaster/blob/main/LICENSE";

        // GitHub Repo URL
        public const string GithubRepoUrl = "https://github.com/AriesLR/PakMaster";

        // GitHub Issues URL
        public const string GithubIssuesUrl = "https://github.com/AriesLR/PakMaster/issues/new?template=issue---pakmaster.md";

        // BuyMeACoffee URL
        public const string BuyMeACoffeeUrl = "https://buymeacoffee.com/arieslr";

        // Patreon URL
        public const string PatreonUrl = "https://www.patreon.com/c/arieslr/membership";

        // ============ Only Edit Below If Necessary ============

        // ============ Helpers ============

        // App License Url Helper
        public static string AppLicenseUrl { get => _appLicenseUrl; set => _appLicenseUrl = value; }

        // App License Uri Helper
        public static Uri? AppLicenseUri => Uri.TryCreate(AppLicenseUrl, UriKind.Absolute, out var uri) ? uri : null;

        // ============ PakMaster Dependencies ============

        public const string RepakUrl = "https://github.com/trumank/repak/releases/download/v0.2.3/repak_cli-x86_64-pc-windows-msvc.zip";

        public const string ZenToolsUrl = "https://github.com/LongerWarrior/ZenTools/releases/download/1.06UE5.1-5.2/ZenTools.exe";
    }
}