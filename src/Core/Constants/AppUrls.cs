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

        // Base Dependencies
        public const string RetocUrl = "https://github.com/trumank/retoc/releases/latest/download/retoc_cli-x86_64-pc-windows-msvc.zip";

        public const string RepakUrl = "https://github.com/trumank/repak/releases/latest/download/repak_cli-x86_64-pc-windows-msvc.zip";

        // Repak Branches
        public const string RepakChunkedCompressionUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-chunked-compression.exe";

        public const string RepakCSBindingsUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-cs-bindings.exe";

        public const string RepakPatchBack4BloodUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-back4blood.exe";

        public const string RepakDeadByDaylightUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-dead-by-daylight.exe";

        public const string RepakDragonQuestXiUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-dragon-quest-xi.exe";

        public const string RepakMarvelRivalsUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-marvel-rivals.exe";

        public const string RepakOutlastTrialsUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-outlast-trials.exe";

        public const string RepakTorchlightUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-torchlight.exe";

        public const string RepakVisionsOfManaUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-visions-of-mana.exe";

        public const string RepakWutheringWavesUrl = "https://github.com/AriesLR/PakMaster-Dependencies/releases/download/latest-branch-builds/repak-patch-wuthering-waves.exe";
    }
}