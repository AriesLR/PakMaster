namespace PakMaster.Core.Models
{
    public class PackageModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string ExecutableName { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty;
        public bool IsInstalled { get; set; } = false;
        public bool PendingInstallState { get; set; } = false;
    }
}
