namespace PakMaster.Core.Models
{
    public class SerializableExceptionModel
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public SerializableExceptionModel? InnerException { get; set; }
    }
}