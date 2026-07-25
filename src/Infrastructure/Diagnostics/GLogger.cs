namespace PakMaster.Infrastructure.Diagnostics
{
    public static class GLogger
    {
        public static Serilog.ILogger Here([System.Runtime.CompilerServices.CallerFilePath] string filePath = "")
        {
            string context = System.IO.Path.GetFileNameWithoutExtension(filePath);
            if (context.EndsWith(".xaml")) context = context.Substring(0, context.Length - 5);

            return Serilog.Log.ForContext("SourceContext", context);
        }
    }
}