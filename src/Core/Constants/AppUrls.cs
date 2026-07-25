namespace PakMaster.Core.Constants
{
    public static class AppUrls
    {
        // ============ Edit This Section ============

        // Update Tracking URL
        // https://raw.githubusercontent.com/AriesLR/PakMaster-Public/refs/heads/main/docs/version/update.json
        public const string UpdateUrl = "https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/version/update.json";

        // App License URL
        // https://github.com/AriesLR/PakMaster-Public/blob/main/LICENSE
        private static string _appLicenseUrl = "https://github.com/AriesLR/PakMaster/blob/main/LICENSE";

        // GitHub Repo URL
        // https://github.com/AriesLR/PakMaster-Public
        public const string GithubRepoUrl = "https://github.com/AriesLR/PakMaster";

        // GitHub Issues URL
        // https://github.com/AriesLR/PakMaster-Public/issues/new?template=issue---project-velocity.md
        public const string GithubIssuesUrl = "https://github.com/AriesLR/Project-Velocity-Public/issues/new?template=issue---project-velocity.md";

        // BuyMeACoffee URL
        // https://buymeacoffee.com/arieslr
        public const string BuyMeACoffeeUrl = "https://buymeacoffee.com/arieslr";

        // Patreon URL
        // https://www.patreon.com/c/arieslr/membership
        public const string PatreonUrl = "https://www.patreon.com/c/arieslr/membership";

        // ============ Only Edit Below If Necessary ============

        // ============ Helpers ============

        // App License Url Helper
        public static string AppLicenseUrl { get => _appLicenseUrl; set => _appLicenseUrl = value; }

        // App License Uri Helper
        public static Uri? AppLicenseUri => Uri.TryCreate(AppLicenseUrl, UriKind.Absolute, out var uri) ? uri : null;
    }
}