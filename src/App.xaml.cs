namespace PakMaster
{
    public partial class App : Application
    {
        // IsLoaded
        public static bool IsLoaded { get; set; } = false;

        private const string AppGuid = AppConfig.AppGuid;
        private static Mutex? _appMutex;
        private static bool _ownsMutex;
        public static ToastManager Toasts { get; } = new();

        private readonly TelemetryManager _telemetry = new();

        private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

        // Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Crash Reporter Mode
            if (e.Args.Length >= 2 && e.Args[0] == "--crash")
            {
                string exceptionJsonPath = e.Args[1];
                base.OnStartup(e);

                AppSettingsManager.Initialize();

                LanguageManager.Initialize(AppSettingsManager.CurrentSettings);

                var crashWindow = new CrashReportWindow(exceptionJsonPath);
                crashWindow.ShowDialog();

                Application.Current.Shutdown();
                return;
            }

            // Single Instance Handling
            _appMutex = new Mutex(true, AppGuid, out bool isNewInstance);
            _ownsMutex = isNewInstance;

            if (!isNewInstance)
            {
                MessageBox.Show(Lang.App_Msg_AlreadyRunning_Desc, Lang.App_Msg_AlreadyRunning_Title, MessageBoxButton.OK, MessageBoxImage.Information);
                _appMutex?.Dispose();
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);

            // Global Exception Handling
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Create Splash Screen
            var splashScreen = new SplashWindow();

            if (DebugConfig.DebugSplashScreen)
            {
                splashScreen.AllowsTransparency = false;
                splashScreen.WindowStyle = WindowStyle.SingleBorderWindow;
                splashScreen.ResizeMode = ResizeMode.CanResize;
                splashScreen.Title = "DESIGN MODE - Splash Screen";
            }

            // Display Splash Screen
            splashScreen.Show();

            if (DebugConfig.LockSplashScreen)
            {
                await Task.Delay(-1);
            }

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Application Initialization
                await Task.Run(async () =>
                {
                    // Report Progress To Splash Screen
                    async Task ReportProgressAsync(string statusText)
                    {
                        splashScreen.UpdateStatus(statusText);
                        await Task.Delay(300);
                    }

                    // Early Settings Load For Logging
                    var initialSettings = AppSettingsManager.LoadAppSettings() ?? new AppSettingsModel();

                    // Init Logging Service
                    await ReportProgressAsync(Lang.App_ReportProgress_InitLogging);
                    LoggingManager.Initialize(initialSettings.EnableDebugLogging, initialSettings);

                    // Init App Settings
                    await ReportProgressAsync(Lang.App_ReportProgress_InitAppSettings);
                    AppSettingsManager.Initialize();

                    // Init Language Manager
                    await ReportProgressAsync(Lang.App_ReportProgress_InitLanguageService);
                    LanguageManager.Initialize(AppSettingsManager.CurrentSettings);

                    // Init Dependency Service
                    if (DebugConfig.Dependency)
                    {
                        await ReportProgressAsync(Lang.App_ReportProgress_InitDepedencyService);
                        await DependencyManager.InitializeAsync();
                    }

                    // Init Theme Builder
                    await ReportProgressAsync(Lang.App_ReportProgress_InitThemeService);
                    await Dispatcher.InvokeSafeAsync(() =>
                    {
                        ThemeBuilder.Initialize(AppSettingsManager.CurrentSettings);
                    });

                    // Init Telemetry Service
                    if (DebugConfig.Telemetry)
                    {
                        await ReportProgressAsync(Lang.App_ReportProgress_InitTelemetryService);
                        await _telemetry.InitializeAsync();
                        await _telemetry.SendAsync(true);
                    }

                    // Final Splash Message
                    await ReportProgressAsync(Lang.App_ReportProgress_Final);

                    splashScreen.LoadingComplete();

                    // Small delay before MainWindow gets created
                    await ReportProgressAsync("You shouldn't see this, but if you do... Hi.");
                });

                // Create MainWindow
                var mainWindow = new MainWindow();
                this.MainWindow = mainWindow;

                // Restore Window Position
                WindowPositionManager.RestorePosition(mainWindow);

                // Flip IsLoaded bool
                mainWindow.ContentRendered += (s, e) =>
                {
                    IsLoaded = true;
                };

                // Show MainWindow and Hide Splash Screen
                mainWindow.Show();
                splashScreen.Close();

                stopwatch.Stop();

                GLogger.Here().Information("Application initialization completed in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);

                // Init Update Service
                await UpdateManager.InitializeAsync();
            }
            catch (Exception ex)
            {
                // Not using logger here because this exception would likely be caused by it not loading properly
                Debug.WriteLine($"Critical startup crash: {ex}");
                MessageBox.Show(Lang.App_Msg_StartupError_Desc, Lang.App_Msg_StartupError_Title, MessageBoxButton.OK, MessageBoxImage.Error);

                splashScreen.Close();
                Application.Current.Shutdown();
            }
        }

        // ============ Global Exception Handling ============

        // App Dispatcher Unhandled Exception
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            GLogger.Here().Fatal(e.Exception, "An unhandled UI thread exception occurred.");
            e.Handled = true;

            ShowCrashWindow(e.Exception);
        }

        // Task Scheduler Unobserved Task Exception
        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            GLogger.Here().Error(e.Exception, "An unobserved background task exception occurred.");

            e.SetObserved();
        }

        // Current Domain Unhandled Exception
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                GLogger.Here().Fatal(ex, "A fatal non-UI AppDomain exception occurred. Terminating: {IsTerminating}", e.IsTerminating);
                ShowCrashWindow(ex);
            }
            else
            {
                GLogger.Here().Fatal("An unknown fatal AppDomain exception occurred.");
                ShowCrashWindow(new Exception("An unknown fatal error occurred in the application domain."));
            }
        }

        // ============ Crash Reporter ============

        // Show Crash Window
        private static void ShowCrashWindow(Exception exception)
        {
            GLogger.Here().Fatal(exception, "The application has encountered a fatal unhandled exception. Initializing crash reporter.");

            try
            {
                var exceptionData = new SerializableExceptionModel
                {
                    Type = exception.GetType().FullName ?? "Exception",
                    Message = exception.Message,
                    StackTrace = exception.StackTrace ?? "No stack trace available.",
                    InnerException = GetInnerExceptionData(exception.InnerException)
                };

                string tempFile = Path.Combine(Path.GetTempPath(), $"Crash_{Guid.NewGuid():N}.json");

                string json = JsonSerializer.Serialize(exceptionData, _serializerOptions);
                File.WriteAllText(tempFile, json, Encoding.UTF8);

                string currentExe = Environment.ProcessPath ?? throw new InvalidOperationException("Failed to determine the current executable path.");
                var startInfo = new ProcessStartInfo(currentExe)
                {
                    Arguments = $"--crash \"{tempFile}\"",
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format(Lang.App_Msg_CrashReporterError_Desc, exception.Message, ex.Message);

                MessageBox.Show(errorMessage, Lang.App_Msg_CrashReporterError_Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        // Get Inner Exception Data
        private static SerializableExceptionModel? GetInnerExceptionData(Exception? inner)
        {
            if (inner == null) return null;
            return new SerializableExceptionModel
            {
                Type = inner.GetType().FullName ?? "Exception",
                Message = inner.Message,
                StackTrace = inner.StackTrace ?? "No stack trace available.",
                InnerException = GetInnerExceptionData(inner.InnerException)
            };
        }

        // ============ Shutdown ============

        // Shutdown
        protected override void OnExit(ExitEventArgs e)
        {
            LoggingManager.Shutdown();

            if (_appMutex != null)
            {
                if (!System.Environment.HasShutdownStarted && _ownsMutex)
                {
                    try
                    {
                        _appMutex.ReleaseMutex();
                    }
                    catch (Exception) { }
                }
                _appMutex.Dispose();
            }

            if (DebugConfig.Telemetry)
            {
                _telemetry.Send(false);
            }

            base.OnExit(e);
        }
    }
}