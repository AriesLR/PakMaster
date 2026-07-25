namespace PakMaster.Core.Models
{
    public class MonitorDetailsModel
    {
        public string DeviceName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int Width => Math.Abs(Right - Left);
        public int Height => Math.Abs(Bottom - Top);
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }
}