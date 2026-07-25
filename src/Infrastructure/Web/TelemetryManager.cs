namespace PakMaster.Infrastructure.Web
{
    public class TelemetryManager
    {
        private static readonly HttpClient _httpClient = new();

        private static readonly string _appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConfig.AppName);
        private static readonly string _guidFilePath = Path.Combine(_appDataFolder, "user.id");

        public string? UserGuid { get; private set; }

        public async Task InitializeAsync()
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                if (!Directory.Exists(_appDataFolder))
                {
                    Directory.CreateDirectory(_appDataFolder);
                }

                if (File.Exists(_guidFilePath))
                {
                    UserGuid = await File.ReadAllTextAsync(_guidFilePath);
                }
                else
                {
                    UserGuid = await FetchGuidFromServerAsync();

                    if (!string.IsNullOrEmpty(UserGuid) && UserGuid != "unknown-user")
                    {
                        await File.WriteAllTextAsync(_guidFilePath, UserGuid);
                    }
                }

                stopwatch.Stop();

                GLogger.Here().Information("Initialized in {ElapsedMilliseconds:F2}ms.", stopwatch.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                UserGuid = "unknown-user";
                GLogger.Here().Error(ex, "Failed to complete initialization. Falling back to default identity.");
            }
        }

        private static async Task<string> FetchGuidFromServerAsync()
        {
            try
            {
                string baseUrl = Encoding.UTF8.GetString(AppConfig.WhyAreYouLookingHere);
                string dynamicToken = GenerateHmacToken(AppConfig.LookSomewhereElse);
                string hwHash = GetHardwareFingerprint();

                var payload = new { hardwareHash = hwHash };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/register");
                request.Headers.Add("x-auth-token", dynamicToken);
                request.Headers.Add("x-app-id", AppConfig.DbSafeAppName);
                request.Content = JsonContent.Create(payload);

                GLogger.Here().Debug("Sending out registration request to remote host.");
                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonDocument = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (jsonDocument.TryGetProperty("guid", out JsonElement guidProperty))
                    {
                        string assignedGuid = guidProperty.GetString() ?? "unknown-user";
                        GLogger.Here().Debug("Server successfully approved registration request.");
                        return assignedGuid;
                    }

                    GLogger.Here().Warning("Server responded with success code but the expected property 'guid' was missing.");
                }
                else
                {
                    GLogger.Here().Warning("Server rejected registration handshake. Status Code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An unhandled failure occurred while processing registration.");
            }

            return "unknown-user";
        }

        private static string GenerateHmacToken(byte[] secretKey)
        {
            string timeString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm");
            byte[] messageBytes = Encoding.UTF8.GetBytes(timeString);

            byte[] hashBytes = HMACSHA256.HashData(secretKey, messageBytes);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private static string GetHardwareFingerprint()
        {
            try
            {
                string cpuCount = Environment.ProcessorCount.ToString();
                string is64Bit = Environment.Is64BitOperatingSystem ? "x64" : "x86";
                string machineName = Environment.MachineName;
                string osVersion = Environment.OSVersion.VersionString;
                string userName = Environment.UserName;
                string machineGuid = GetMachineGuid();

                string rawSignature = $"{machineGuid}:{machineName}:{userName}:{osVersion}:{cpuCount}:{is64Bit}";

                byte[] signatureBytes = Encoding.UTF8.GetBytes(rawSignature);
                byte[] hashBytes = SHA256.HashData(signatureBytes);

                string fingerprint = Convert.ToHexString(hashBytes).ToLowerInvariant();
                return fingerprint;
            }
            catch (Exception ex)
            {
                GLogger.Here().Warning(ex, "Unable to compute device hardware fingerprint. Falling back to default.");
                return "fallback-hw-fingerprint";
            }
        }

        private static string GetMachineGuid()
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                using var cryptoKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                return cryptoKey?.GetValue("MachineGuid")?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                GLogger.Here().Warning(ex, "Failed to read MachineGuid value from registry.");
                return string.Empty;
            }
        }

        public async Task SendAsync(bool isOpen)
        {
            if (string.IsNullOrEmpty(UserGuid) || UserGuid == "unknown-user")
            {
                await InitializeAsync();
            }

            if (UserGuid == "unknown-user")
            {
                GLogger.Here().Warning("Aborting heartbeat transmission; identity is unknown.");
                return;
            }

            try
            {
                var payload = new { guid = UserGuid, isOpen };
                string baseUrl = Encoding.UTF8.GetString(AppConfig.WhyAreYouLookingHere);
                string dynamicToken = GenerateHmacToken(AppConfig.LookSomewhereElse);

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/heartbeat");
                request.Headers.Add("x-auth-token", dynamicToken);
                request.Headers.Add("x-app-id", AppConfig.DbSafeAppName);
                request.Content = JsonContent.Create(payload);

                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    GLogger.Here().Warning("Remote server did not accept telemetry data. Status Code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to send heartbeat.");
            }
        }

        public void Send(bool isOpen)
        {
            if (string.IsNullOrEmpty(UserGuid) || UserGuid == "unknown-user")
            {
                GLogger.Here().Warning("Aborting heartbeat transmission; identity is unknown.");
                return;
            }

            try
            {
                var payload = new { guid = UserGuid, isOpen };
                string baseUrl = Encoding.UTF8.GetString(AppConfig.WhyAreYouLookingHere);
                string dynamicToken = GenerateHmacToken(AppConfig.LookSomewhereElse);

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/heartbeat");
                request.Headers.Add("x-auth-token", dynamicToken);
                request.Headers.Add("x-app-id", AppConfig.DbSafeAppName);
                request.Content = JsonContent.Create(payload);

                using var response = _httpClient.Send(request);

                if (!response.IsSuccessStatusCode)
                {
                    GLogger.Here().Warning("Remote server did not accept telemetry data. Status Code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to send heartbeat.");
            }
        }
    }
}