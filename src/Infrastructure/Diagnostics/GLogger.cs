namespace PakMaster.Infrastructure.Diagnostics
{
    public static class GLogger
    {
        public static ILogger Here([CallerFilePath] string filePath = "")
        {
            string context = Path.GetFileNameWithoutExtension(filePath);
            if (context.EndsWith(".xaml")) context = context.Substring(0, context.Length - 5);

            return Log.ForContext("SourceContext", context);
        }
    }
}