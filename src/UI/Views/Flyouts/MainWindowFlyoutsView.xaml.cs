namespace PakMaster.UI.Views.Flyouts
{
    public partial class MainWindowFlyoutsView : FlyoutsControl
    {
        public MainWindowFlyoutsView()
        {
            InitializeComponent();

            AccentColorDropdown.ItemsSource = ThemeBuilder.AvailableAccents;
            LanguageDropdown.ItemsSource = LanguageManager.SupportedLanguages;

            this.Loaded += MainWindowFlyoutsView_Loaded;
        }

        private void MainWindowFlyoutsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettingsManager.CurrentSettings != null)
            {
                SyncUISelections();

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

            // Sync Accent Color Dropdown (Blue/Lime/Indigo/etc.)
            AccentColorDropdown.SelectedItem = ThemeBuilder.AvailableAccents.FirstOrDefault(t => string.Equals(t.ColorScheme, settings.AccentColor, StringComparison.OrdinalIgnoreCase));

            // Sync Interface Scale Slider
            InterfaceScaleSlider.Value = settings.InterfaceScale;

            // Sync Language Dropdown
            LanguageDropdown.SelectedItem = LanguageManager.SupportedLanguages.FirstOrDefault(lang => string.Equals(lang.CultureCode, settings.Language, StringComparison.OrdinalIgnoreCase)) ?? LanguageManager.SupportedLanguages[0];
        }
    }
}