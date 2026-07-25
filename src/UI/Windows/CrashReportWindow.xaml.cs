namespace PakMaster
{
    public partial class CrashReportWindow : MetroWindow
    {
        private string? _logFilePath;

        public CrashReportWindow(string exceptionJsonPath)
        {
            InitializeComponent();

            this.Loaded += async (s, e) =>
            {
                // Get Latest Log File Path and Create Scrubbed Copy
                string rawLogPath = GetLatestLogFilePath();
                _logFilePath = await CreateScrubbedLogCopyAsync(rawLogPath);

                if (!string.IsNullOrEmpty(_logFilePath))
                {
                    LogPathTextBox.Text = Path.GetFullPath(_logFilePath);
                }
                else
                {
                    LogPathTextBox.Text = "No active log file found in the log directory.";
                }

                // Load Exception Data
                await LoadExceptionDataAsync(exceptionJsonPath);
            };
        }

        // Get Latest Log File Path
        private static string GetLatestLogFilePath()
        {
            try
            {
                string logDirectory = AppConfig.AppLogsFolder;

                if (Directory.Exists(logDirectory))
                {
                    var directoryInfo = new DirectoryInfo(logDirectory);

                    var latestLogFile = directoryInfo.GetFiles("*.log")
                        .Where(f => !f.Name.EndsWith("_scrubbed.log", StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(f => f.LastWriteTime)
                        .FirstOrDefault();

                    if (latestLogFile != null)
                    {
                        return latestLogFile.FullName;
                    }
                }
            }
            catch (Exception ex)
            {
                // TraceError for logging since the app is actively crashing and serilog won't help here
                Trace.TraceError($"Failed to retrieve latest log file path: {ex.Message}");
            }

            return string.Empty;
        }

        // Create Scrubbed Log Copy
        private static async Task<string> CreateScrubbedLogCopyAsync(string rawLogPath)
        {
            if (string.IsNullOrEmpty(rawLogPath) || !File.Exists(rawLogPath))
            {
                return string.Empty;
            }

            try
            {
                string directory = Path.GetDirectoryName(rawLogPath) ?? string.Empty;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(rawLogPath);
                string extension = Path.GetExtension(rawLogPath);

                string scrubbedPath = Path.Combine(directory, $"{fileNameWithoutExtension}_scrubbed{extension}");

                string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                string logContent;
                using var fileStream = new FileStream(rawLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
                logContent = await streamReader.ReadToEndAsync();

                if (!string.IsNullOrEmpty(userProfilePath))
                {
                    logContent = ReplaceCaseInsensitive(logContent, userProfilePath, "%USERPROFILE%");
                }

                await File.WriteAllTextAsync(scrubbedPath, logContent, Encoding.UTF8);

                return scrubbedPath;
            }
            catch (Exception ex)
            {
                // TraceError for logging since the app is actively crashing and serilog won't help here
                Trace.TraceError($"Failed to scrub log file: {ex}");
                return rawLogPath;
            }
        }

        // Replace Case Insensitive
        private static string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                input,
                System.Text.RegularExpressions.Regex.Escape(search),
                replacement,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        // Load Exception Data
        private async Task LoadExceptionDataAsync(string jsonPath)
        {
            try
            {
                if (File.Exists(jsonPath))
                {
                    string json = await File.ReadAllTextAsync(jsonPath, Encoding.UTF8);
                    var exceptionData = JsonSerializer.Deserialize<SerializableExceptionModel>(json);

                    if (exceptionData != null)
                    {
                        ExceptionMessageTextBlock.Text = exceptionData.Message;
                        StackTraceTextBox.Text = FormatSerializedException(exceptionData);
                    }

                    File.Delete(jsonPath);
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageTextBlock.Text = "Failed to load exception details.";
                StackTraceTextBox.Text = $"An error occurred while reading the crash dump details:\n{ex.Message}";
            }
        }

        // Format Serialized Exception
        private static string FormatSerializedException(SerializableExceptionModel exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Exception:\n{exception.Type}\n");
            sb.AppendLine($"Message:\n{exception.Message}\n");
            sb.AppendLine($"Stack Trace:\n{exception.StackTrace}");

            SerializableExceptionModel? inner = exception.InnerException;
            int depth = 1;
            while (inner != null)
            {
                sb.AppendLine($"\n{new string('=', 40)}");
                sb.AppendLine($"Inner Exception #{depth}:\n{inner.Type}\n");
                sb.AppendLine($"Message:\n{inner.Message}\n");
                sb.AppendLine($"Stack Trace:\n{inner.StackTrace}");
                inner = inner.InnerException;
                depth++;
            }
            return sb.ToString();
        }

        // ============ Button Clicks ============

        // Close Button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Submit Button
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string githubIssuesUrl = AppUrls.GithubIssuesUrl;

                Process.Start(new ProcessStartInfo(githubIssuesUrl) { UseShellExecute = true });

                if (!string.IsNullOrEmpty(_logFilePath) && File.Exists(_logFilePath))
                {
                    string fullPath = Path.GetFullPath(_logFilePath);
                    Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
                }
                else
                {
                    string logDirectory = AppConfig.AppLogsFolder;
                    if (Directory.Exists(logDirectory))
                    {
                        Process.Start("explorer.exe", $"\"{Path.GetFullPath(logDirectory)}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format(Lang.CrashReportWindow_Msg_SubmitError_Desc, _logFilePath, ex.Message);

                MessageBox.Show(errorMessage, Lang.Msg_Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}