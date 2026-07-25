namespace PakMaster.Core.Models
{
    public class DependencyTrackStateModel
    {
        public string Key { get; set; } = string.Empty;
        public DateTime InstalledAt { get; set; } = DateTime.UtcNow;
    }
}