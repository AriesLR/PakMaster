namespace PakMaster.Infrastructure.Diagnostics
{
    public static class LoggingManager
    {
        private static readonly LoggingLevelSwitch _levelSwitch = new();

        // Initialize Logging Service
        public static void Initialize(bool isDebugEnabled, object settings)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            _levelSwitch.MinimumLevel = isDebugEnabled
                ? Serilog.Events.LogEventLevel.Debug
                : Serilog.Events.LogEventLevel.Fatal;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_levelSwitch)
                .WriteTo.File(
                    path: Path.Combine(AppConfig.AppLogsFolder, "debug-.log"),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 25 * 1024 * 1024,  // 25MB
                    retainedFileCountLimit: 2,           // Keep last 2 day(s) of logs
                    rollOnFileSizeLimit: true,
                    hooks: new HeaderWriterFileHook(settings)
                )
                .CreateLogger();

            stopwatch.Stop();

            GLogger.Here().Information("Initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
        }

        // Toggle Logging State
        public static void SetLoggingState(bool enable)
        {
            _levelSwitch.MinimumLevel = enable
                ? Serilog.Events.LogEventLevel.Debug
                : Serilog.Events.LogEventLevel.Fatal;

            if (enable)
            {
                GLogger.Here().Debug("Debug logging has been manually enabled by the user.");
            }
        }

        // Shutdown Logger
        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }

    public class HeaderWriterFileHook(object settings) : Serilog.Sinks.File.FileLifecycleHooks
    {
        public override Stream OnFileOpened(string path, Stream underlyingStream, Encoding encoding)
        {
            if (underlyingStream.Length == 0)
            {
                var headerText = SystemProfiler.GenerateLogHeader(settings) + Environment.NewLine;
                var headerBytes = encoding.GetBytes(headerText);
                underlyingStream.Write(headerBytes, 0, headerBytes.Length);
            }
            return underlyingStream;
        }
    }
}