namespace PakMaster.Core.Models
{
    public class LanguageModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string CultureCode { get; set; } = string.Empty;

        public CultureInfo Culture => new(CultureCode);
    }
}