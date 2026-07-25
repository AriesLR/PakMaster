namespace PakMaster.Core.Models
{
    public class CustomToastModel
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public PackIconMaterialKind IconKind { get; set; }
        public Brush? ColorBrush { get; set; }
    }
}
