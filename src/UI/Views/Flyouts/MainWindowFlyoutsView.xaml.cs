namespace PakMaster.UI.Views.Flyouts
{
    public partial class MainWindowFlyoutsView : FlyoutsControl
    {
        private bool _isInitialized = true;

        public MainWindowFlyoutsView()
        {
            InitializeComponent();

            //BaseThemeDropdown.ItemsSource = ThemeBuilder.AvailableBaseThemes;
            AccentColorDropdown.ItemsSource = ThemeBuilder.AvailableAccents;
            LanguageDropdown.ItemsSource = LanguageManager.SupportedLanguages;

            this.Loaded += MainWindowFlyoutsView_Loaded;
        }

        private void MainWindowFlyoutsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettingsManager.CurrentSettings != null)
            {
                SyncUISelections();

                LoadRepakVersionInfo();
                LoadAesKeysAsync();
                LoadUnrealPakConfigAsync();

                InterfaceScaler.ApplyInterfaceScale(AppSettingsManager.CurrentSettings.InterfaceScale);
            }
        }

        // ============ Button Clicks ============

        // Hyperlink Click
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            UrlOperations.OpenUrlAsync(e.Uri.AbsoluteUri);

            e.Handled = true;
        }

        // Open Buy Me A Coffee Button
        private void OpenBuyMeACoffee_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Information("User clicked the Buy Me A Coffee button.");
            UrlOperations.OpenUrlAsync(AppUrls.BuyMeACoffeeUrl);
        }

        // Open Patreon Button
        private void OpenPatreon_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Information("User clicked the Patreon button.");
            UrlOperations.OpenUrlAsync(AppUrls.PatreonUrl);
        }

        // Check For App Updates Button
        private async void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Information("User clicked the Check for Updates button.");
            await UpdateManager.CheckForUpdatesAsync(AppUrls.UpdateUrl);
        }

        // App Settings Button
        private void AppSettingsFlyout_Click(object? sender, RoutedEventArgs e)
        {
            GLogger.Here().Information("User clicked the App Settings button.");
            ((MainWindowState)DataContext).OpenAppSettingsFlyout();
        }

        // Open Logs Folder Button
        private async void OpenLogsFolder_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Information("User clicked the Open Logs Folder button.");
            await FolderOperations.OpenFolderAsync(AppConfig.AppLogsFolder);
        }

        // Open App Config Folder Button
        private async void OpenAppConfigFolder_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Information("User clicked the Open App Config Folder button.");
            await FolderOperations.OpenFolderAsync(AppConfig.AppConfigFolder);
        }

        // Factory Reset Settings Button
        private async void ResetToFactorySettings_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Information("User clicked the Factory Reset Settings button.");

            bool confirmReset = await MessageManager.ShowYesNo(Lang.MainWindowFlyoutsView_Msg_ResetToFactorySettings_Title, Lang.MainWindowFlyoutsView_Msg_ResetToFactorySettings_Desc);

            if (!confirmReset)
            {
                return;
            }

            bool success = AppSettingsManager.ResetToFactoryDefaults();

            if (success)
            {
                SyncUISelections();

                await MessageManager.ShowInfo(Lang.MainWindowFlyoutsView_Msg_ResetToFactorySettingsComplete_Title, Lang.MainWindowFlyoutsView_Msg_ResetToFactorySettingsComplete_Desc);
            }
            else
            {
                await MessageManager.ShowError(Lang.MainWindowFlyoutsView_Msg_ResetToFactorySettingsError_Desc);
            }
        }

        // Reset Window Position
        private void ResetWindowPosition_Click(object sender, RoutedEventArgs e)
        {
            WindowPositionManager.ResetWindowPosition();
        }

        // IoStore Package Button
        private async void IoStorePackage_Click(object sender, RoutedEventArgs e)
        {
            await UnrealPakEngine.RepackAsync(output =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    IoStoreCommandOutputTextBox.Text += output + Environment.NewLine;
                });
            });
        }

        // ============ Event Handlers ============

        // Check App Updates Toggled
        private async void CheckAppUpdatesToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null) return;

            bool isOn = CheckAppUpdatesToggle.IsOn;
            GLogger.Here().Information("User flipped the Check App Updates Toggle to: {State}", isOn);

            AppSettingsManager.CurrentSettings.CheckForUpdatesOnStartup = isOn;
            AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            await App.Toasts.ShowConfigSaved();
        }

        // Start With Windows Toggled
        private async void StartWithWindowsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null)
                return;

            bool isOn = StartWithWindowsToggle.IsOn;
            GLogger.Here().Information("User flipped the Start With Windows Toggle to: {State}", isOn);

            AppSettingsManager.CurrentSettings.StartWithWindows = isOn;
            AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            await App.Toasts.ShowConfigSaved();

            try
            {
                StartupRegistry.SetStartWithWindows(AppConfig.AppName, isOn);
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to update registry startup key.");

                string messageDesc = string.Format(
                    Lang.MainWindowFlyoutsView_Msg_StartWithWindowsToggleError_Desc,
                    ex.Message
                );

                await MessageManager.ShowError(messageDesc);
            }
        }

        // Always On Top Toggled
        private async void AlwaysOnTopToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null) return;

            bool isOn = AlwaysOnTopToggle.IsOn;
            GLogger.Here().Information("User flipped the Always on Top Toggle to: {Status}", isOn);

            AppSettingsManager.CurrentSettings.AlwaysOnTop = isOn;
            AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            await App.Toasts.ShowConfigSaved();

            if (Application.Current.MainWindow is Window mainWindow)
            {
                mainWindow.Topmost = isOn;
                GLogger.Here().Debug("MainWindow Topmost property set to: {State}", isOn);
            }
        }

        // Minimize To Tray Toggled
        private async void MinimizeToTrayToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null) return;

            bool isOn = MinimizeToTrayToggle.IsOn;

            AppSettingsManager.CurrentSettings.MinimizeToTray = isOn;
            AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            await App.Toasts.ShowConfigSaved();

            TrayIconManager.SetVisibility(isOn);

            GLogger.Here().Information("User flipped the Minimize to System Tray Toggle to: {Status}", isOn);
        }

        // Enable Debug Logging Toggled
        private async void EnableDebugLoggingToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null) return;

            bool isOn = EnableDebugLoggingToggle.IsOn;

            LoggingManager.SetLoggingState(isOn);
            AppSettingsManager.CurrentSettings.EnableDebugLogging = isOn;
            AppSettingsManager.SaveAppSettings(AppSettingsManager.CurrentSettings);
            await App.Toasts.ShowConfigSaved();

            GLogger.Here().Information("User flipped the Enable Debug Logging Toggle to: {Status}", isOn);
        }

        // Base Theme Selection Changed
        /*private void BaseThemeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded || BaseThemeDropdown.SelectedItem is not string selectedBase)
                return;

            GLogger.Here().Information("User modified base theme to: {Theme}", selectedBase);
            ThemeBuilder.SetBaseTheme(selectedBase);
            App.Toasts.ShowConfigSaved();
        }*/

        // Language Selection Changed
        private async void LanguageDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!App.IsLoaded || LanguageDropdown.SelectedItem is not LanguageModel selectedLanguage)
                return;

            GLogger.Here().Information("User modified interface language to: {Language}", selectedLanguage.DisplayName);
            LanguageManager.SetLanguage(selectedLanguage.CultureCode);
            await App.Toasts.ShowConfigSaved();
        }

        // Accent Color Selection Changed
        private async void AccentColorDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!App.IsLoaded || AccentColorDropdown.SelectedItem is not Theme selectedAccent)
                return;

            GLogger.Here().Information("User modified accent color to: {Accent}", selectedAccent.ColorScheme);
            ThemeBuilder.SetAccentColor(selectedAccent.ColorScheme);
            await App.Toasts.ShowConfigSaved();
        }

        // Interface Scale Slider Value Changed
        private async void InterfaceScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null) return;

            double roundedValue = Math.Round(e.NewValue);

            if (Math.Abs(InterfaceScaleSlider.Value - roundedValue) > 0.001)
            {
                InterfaceScaleSlider.Value = roundedValue;
                return;
            }

            if (InterfaceScaleSlider.IsMouseCaptureWithin) return;

            await InterfaceScaler.ExecuteScaleUpdate((int)roundedValue, saveAndNotify: true);
        }

        // Interface Scale Slider Drag Completed
        private async void InterfaceScaleSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!App.IsLoaded || AppSettingsManager.CurrentSettings == null) return;

            double roundedValue = Math.Round(InterfaceScaleSlider.Value);

            if (Math.Abs(InterfaceScaleSlider.Value - roundedValue) > 0.001)
            {
                InterfaceScaleSlider.Value = roundedValue;
                return;
            }

            await InterfaceScaler.ExecuteScaleUpdate((int)roundedValue, saveAndNotify: true);
        }

        // ============ UI Helpers ============

        // Sync UI Elements
        private void SyncUISelections()
        {
            var settings = AppSettingsManager.CurrentSettings;
            if (settings == null) return;

            GLogger.Here().Debug("Synchronizing UI elements.");

            // Sync UI Toggles
            StartWithWindowsToggle.IsOn = settings.StartWithWindows;
            AlwaysOnTopToggle.IsOn = settings.AlwaysOnTop;
            MinimizeToTrayToggle.IsOn = settings.MinimizeToTray;

            CheckAppUpdatesToggle.IsOn = settings.CheckForUpdatesOnStartup;
            EnableDebugLoggingToggle.IsOn = settings.EnableDebugLogging;

            // Sync Base Theme Dropdown (Light/Dark)
            //BaseThemeDropdown.SelectedItem = settings.BaseTheme;

            // Sync Accent Color Dropdown (Blue/Lime/Indigo/etc.)
            AccentColorDropdown.SelectedItem = ThemeBuilder.AvailableAccents.FirstOrDefault(t => string.Equals(t.ColorScheme, settings.AccentColor, StringComparison.OrdinalIgnoreCase));

            // Sync Interface Scale Slider
            InterfaceScaleSlider.Value = settings.InterfaceScale;

            // Sync Language Dropdown
            LanguageDropdown.SelectedItem = LanguageManager.SupportedLanguages.FirstOrDefault(lang => string.Equals(lang.CultureCode, settings.Language, StringComparison.OrdinalIgnoreCase)) ?? LanguageManager.SupportedLanguages[0];
        }

        // ============ TEMP ============

        // Load UnrealPak Config
        private async void LoadUnrealPakConfigAsync()
        {
            try
            {
                var unrealPakConfig = ConfigManager.CurrentSettings.UnrealPak;

                string unrealPakPath = unrealPakConfig.UnrealPakPath;
                string globalOutputPath = unrealPakConfig.GlobalOutputPath;
                string cookedFilesPath = unrealPakConfig.CookedFilesPath;
                string packageStorePath = unrealPakConfig.PackageStorePath;
                string scriptObjectsPath = unrealPakConfig.ScriptObjectsPath;
                string ioStoreCommandsPath = unrealPakConfig.IoStoreCommandsPath;

                UnrealPakPathTextBox.Text = unrealPakPath;
                GlobalOutputPathTextBox.Text = globalOutputPath;
                CookedFilesPathTextBox.Text = cookedFilesPath;
                PackageStorePathTextBox.Text = packageStorePath;
                ScriptObjectsPathTextBox.Text = scriptObjectsPath;
                IoStoreCommandsPathTextBox.Text = ioStoreCommandsPath;
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error loading UnrealPak config: {ex.Message}");
            }
        }

        // Load AES Key
        private async void LoadAesKeysAsync()
        {
            try
            {
                var config = ConfigManager.CurrentSettings;
                string aesKey = config.Repak.AesKey;

                var zenToolsConfig = ConfigManager.LoadZenToolsConfig();
                string zenToolsKeyGuid = string.Empty;
                string zenToolsKeyHex = string.Empty;

                if (zenToolsConfig != null)
                {
                    foreach (var kvp in zenToolsConfig)
                    {
                        zenToolsKeyGuid = kvp.Key;
                        zenToolsKeyHex = kvp.Value;
                        break; // Just get the first one
                    }
                }

                AesKeyTextBox.Text = aesKey;
                ZenToolsKeyGuidTextBox.Text = zenToolsKeyGuid;
                ZenToolsKeyHexTextBox.Text = zenToolsKeyHex;
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error loading config: {ex.Message}");
            }
        }

        // Save Repak AES Key
        private async void SaveRepakConfigAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                string aesKey = AesKeyTextBox.Text.Trim();

                var config = ConfigManager.CurrentSettings;
                config.Repak.AesKey = aesKey;
                ConfigManager.SaveConfig(config);

                await MessageManager.ShowInfo("Success", "Repak configuration saved successfully!");
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error saving Repak AES Keys config: {ex.Message}");
            }
        }

        // Save ZenTools AES Key
        private async void SaveZenToolsConfigAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                string zenToolsKeyGuid = ZenToolsKeyGuidTextBox.Text.Trim();
                string zenToolsKeyHex = ZenToolsKeyHexTextBox.Text.Trim();

                ConfigManager.SaveZenToolsConfig(zenToolsKeyGuid, zenToolsKeyHex);

                await MessageManager.ShowInfo("Success", "ZenTools configuration saved successfully!");
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error saving ZenTools AES Keys config: {ex.Message}");
            }
        }

        // Browse global output path
        private void BrowseGlobalOutputPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select global.utoc Output Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            var config = ConfigManager.CurrentSettings;
            string lastPath = config.UnrealPak.GlobalOutputPath;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = lastPath;
            }

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    config.UnrealPak.GlobalOutputPath = selectedPath;
                    ConfigManager.SaveConfig(config);
                    GlobalOutputPathTextBox.Text = selectedPath;
                }
            }
        }

        // Browse cooked files path
        private void BrowseCookedFilesPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Folder Containing Cooked Files",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            var config = ConfigManager.CurrentSettings;
            string lastPath = config.UnrealPak.CookedFilesPath;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = lastPath;
            }

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    config.UnrealPak.CookedFilesPath = selectedPath;
                    ConfigManager.SaveConfig(config);
                    CookedFilesPathTextBox.Text = selectedPath;
                }
            }
        }

        // Browse packagestore.manifest path
        private void BrowsePackageStorePath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Package Store File (*.manifest, *json)|*.manifest;*.json|All Files (*.*)|*.*",
                Title = "Select Package Store File"
            };

            var config = ConfigManager.CurrentSettings;
            string lastPath = config.UnrealPak.PackageStorePath;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.UnrealPak.PackageStorePath = dialog.FileName;
                ConfigManager.SaveConfig(config);
                PackageStorePathTextBox.Text = dialog.FileName;
            }
        }

        // Browse ScriptObjects.bin path
        private void BrowseScriptObjectsPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "ScriptObjects.bin|ScriptObjects.bin|All Files (*.*)|*.*",
                Title = "Select ScriptObjects.bin File"
            };

            var config = ConfigManager.CurrentSettings;
            string lastPath = config.UnrealPak.ScriptObjectsPath;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.UnrealPak.ScriptObjectsPath = dialog.FileName;
                ConfigManager.SaveConfig(config);
                ScriptObjectsPathTextBox.Text = dialog.FileName;
            }
        }

        // Browse IoStoreCommands.txt path
        private void BrowseIoStoreCommandsPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Commands (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Select Commands.txt File"
            };

            var config = ConfigManager.CurrentSettings;
            string lastPath = config.UnrealPak.IoStoreCommandsPath;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.UnrealPak.IoStoreCommandsPath = dialog.FileName;
                ConfigManager.SaveConfig(config);
                IoStoreCommandsPathTextBox.Text = dialog.FileName;
            }
        }

        private async void OpenCryptoKeysFileAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                bool userConfirmed = await MessageManager.ShowYesNo("Warning", "Are you sure you want to open Crypto.json?\n\nOnly edit this file if you know what you're doing.");

                if (userConfirmed)
                {
                    string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string filePath = Path.Combine(appDirectory, "configs", "Crypto.json");

                    if (File.Exists(filePath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        await MessageManager.ShowError("Crypto.json file not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageManager.ShowError($"Error opening file: {ex.Message}");
            }
        }

        // Repak Version Switch Dropdown

        private void ComboBox_RepakVersion(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (RepakVersionSwitchDropdown.SelectedItem is ComboBoxItem selectedItem)
            {
                var config = ConfigManager.CurrentSettings;
                config.Repak.RepakVersion = selectedItem.Content?.ToString() ?? string.Empty;
                ConfigManager.SaveConfig(config);
            }
        }

        // Repak Settings Version Info
        public void LoadRepakVersionInfo()
        {
            List<RepakVersionInfoModel> repakVersionInfo =
            [
                new RepakVersionInfoModel { UEVersion = "", Version = "1", VersionFeature = "Initial", Read = "?", Write = "?" },
                new RepakVersionInfoModel { UEVersion = "4.0-4.2", Version = "2", VersionFeature = "NoTimestamps", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "4.3-4.15", Version = "3", VersionFeature = "CompressionEncryption", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "4.16-4.19", Version = "4", VersionFeature = "IndexEncryption", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "4.20", Version = "5", VersionFeature = "RelativeChunkOffsets", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "", Version = "6", VersionFeature = "DeleteRecords", Read = "?", Write = "?" },
                new RepakVersionInfoModel { UEVersion = "4.21", Version = "7", VersionFeature = "EncryptionKeyGuid", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "4.22", Version = "8A", VersionFeature = "FNameBasedCompression", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "4.23-4.24", Version = "8B", VersionFeature = "FNameBasedCompression", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "4.25", Version = "9", VersionFeature = "FrozenIndex", Read = "✔", Write = "✔" },
                new RepakVersionInfoModel { UEVersion = "", Version = "10", VersionFeature = "PathHashIndex", Read = "?", Write = "?" },
                new RepakVersionInfoModel { UEVersion = "4.26-5.3", Version = "11", VersionFeature = "Fnv64BugFix", Read = "✔", Write = "✔" }
            ];

            RepakDataGrid.ItemsSource = repakVersionInfo;

            var repakConfig = ConfigManager.CurrentSettings.Repak;
            string repakVersion = repakConfig.RepakVersion;

            if (!string.IsNullOrEmpty(repakVersion))
            {
                var items = RepakVersionSwitchDropdown.Items;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] is ComboBoxItem item && item.Content?.ToString() == repakVersion)
                    {
                        RepakVersionSwitchDropdown.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        // Open IoStore Flyout
        private void OpenIoStoreFlyout(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowState viewModel)
            {
                viewModel.OpenIoStoreFlyout();
                LoadUnrealPakConfigAsync(); // Load config for unrealpak paths
            }
        }

        // Open AesKeys Flyout (Settings/Config)
        private void OpenAesKeysFlyout(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowState viewModel)
            {
                viewModel.OpenAesKeysFlyout();
                LoadAesKeysAsync(); // Load again here in case user changes the values via the config directly.
            }
        }

        // Browse UnrealPak executable
        private void BrowseUnrealPakPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "UnrealPak.exe|UnrealPak.exe|All Files (*.*)|*.*",
                Title = "Select UnrealPak Executable"
            };

            var config = ConfigManager.CurrentSettings;
            string lastPath = config.UnrealPak.UnrealPakPath;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.UnrealPak.UnrealPakPath = dialog.FileName;
                ConfigManager.SaveConfig(config);
                UnrealPakPathTextBox.Text = dialog.FileName;
            }
        }

        private void CliOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            IoStoreCommandOutputTextBox.ScrollToEnd();
        }
    }
}