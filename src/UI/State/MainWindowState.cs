namespace PakMaster.UI.State
{
    public class MainWindowState : INotifyPropertyChanged, IDisposable
    {
        private bool _isAppSettingsFlyoutOpen;
        private bool _isAboutFlyoutOpen;
        private bool _isAesKeysFlyoutOpen;
        private bool _isIoStoreFlyoutOpen;
        private bool _isRepakSettingsFlyoutOpen;

        public ObservableCollection<SidebarModel> MenuItems { get; set; }
        public ObservableCollection<SidebarModel> OptionsMenuItems { get; set; }

        public MainWindowState()
        {
            LanguageManager.LanguageChanged += OnLanguageChanged;

            // Top Menu Items
            MenuItems =
            [
                new SidebarModel
                {
                    TextGetter = () => Lang.AESKeys_Title,
                    Icon = "ShieldKey",
                    Command = new RelayCommand((sender, e) => SidebarAESKeys_Click(sender, e))
                },
                new SidebarModel
                {
                    TextGetter = () => Lang.RepakSettings_Title,
                    Icon = "AlphaRBox",
                    Command = new RelayCommand((sender, e) => SidebarRepakSettings_Click(sender, e))
                },
                new SidebarModel
                {
                    TextGetter = () => Lang.ZenToolsSettings_Title,
                    Icon = "AlphaZBox",
                    Command = new RelayCommand((sender, e) => SidebarZenToolsSettings_Click(sender, e))
                }
            ];

            // Bottom Menu Items
            OptionsMenuItems =
            [
                new SidebarModel
                {
                    TextGetter = () => Lang.AppSettings_Title,
                    Icon = "Cog",
                    Command = new RelayCommand((sender, e) => SidebarAppSettings_Click(sender, e))
                },
                new SidebarModel
                {
                    TextGetter = () => Lang.About_Title,
                    Icon = "InformationOutline",
                    Command = new RelayCommand((sender, e) => SidebarAbout_Click(sender, e))
                }
            ];
        }

        // ============ Legacy Flyouts ============
        private void SidebarAESKeys_Click(object? sender, RoutedEventArgs e)
        {
            OpenAesKeysFlyout();
        }

        private void SidebarRepakSettings_Click(object? sender, RoutedEventArgs e)
        {
            OpenRepakSettingsFlyout();
        }

        private void SidebarZenToolsSettings_Click(object? sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("This feature is not finished.", "Work In Progress", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public bool IsAesKeysFlyoutOpen
        {
            get => _isAesKeysFlyoutOpen;
            set
            {
                if (_isAesKeysFlyoutOpen != value)
                {
                    _isAesKeysFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public void OpenAesKeysFlyout()
        {
            if (IsAesKeysFlyoutOpen) IsAesKeysFlyoutOpen = false;
            IsAesKeysFlyoutOpen = true;
        }

        public bool IsIoStoreFlyoutOpen
        {
            get => _isIoStoreFlyoutOpen;
            set
            {
                if (_isIoStoreFlyoutOpen != value)
                {
                    _isIoStoreFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public void OpenIoStoreFlyout()
        {
            if (IsIoStoreFlyoutOpen) IsIoStoreFlyoutOpen = false;
            IsIoStoreFlyoutOpen = true;
        }

        public bool IsRepakSettingsFlyoutOpen
        {
            get => _isRepakSettingsFlyoutOpen;
            set
            {
                if (_isRepakSettingsFlyoutOpen != value)
                {
                    _isRepakSettingsFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public void OpenRepakSettingsFlyout()
        {
            if (IsRepakSettingsFlyoutOpen) IsRepakSettingsFlyoutOpen = false;
            IsRepakSettingsFlyoutOpen = true;
        }

        // ============ App Settings Flyout ============
        private void SidebarAppSettings_Click(object? sender, RoutedEventArgs e)
        {
            OpenAppSettingsFlyout();
        }

        public bool IsAppSettingsFlyoutOpen
        {
            get => _isAppSettingsFlyoutOpen;
            set
            {
                if (_isAppSettingsFlyoutOpen != value)
                {
                    _isAppSettingsFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public void OpenAppSettingsFlyout()
        {
            if (IsAppSettingsFlyoutOpen)
            {
                IsAppSettingsFlyoutOpen = false;
            }
            IsAppSettingsFlyoutOpen = true;
        }

        // ============ About Flyout ============
        private void SidebarAbout_Click(object? sender, RoutedEventArgs e)
        {
            OpenAboutFlyout();
        }

        public bool IsAboutFlyoutOpen
        {
            get => _isAboutFlyoutOpen;
            set
            {
                if (_isAboutFlyoutOpen != value)
                {
                    _isAboutFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public void OpenAboutFlyout()
        {
            if (IsAboutFlyoutOpen)
            {
                IsAboutFlyoutOpen = false;
            }
            IsAboutFlyoutOpen = true;
        }

        // ============ Property/Language Changed Helpers ============
        public void OnLanguageChanged(object? sender = null, EventArgs? e = null)
        {
            foreach (var item in MenuItems)
            {
                item.UpdateText();
            }

            foreach (var item in OptionsMenuItems)
            {
                item.UpdateText();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            LanguageManager.LanguageChanged -= OnLanguageChanged;
        }
    }

    // Sidebar Helper Classes
    public class RelayCommand(Action<object?, RoutedEventArgs> execute, Func<object?, bool>? canExecute = null) : ICommand
    {
        private readonly Action<object?, RoutedEventArgs> _execute = execute;
        private readonly Func<object?, bool> _canExecute = canExecute ?? (param => true);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute(parameter);

        public void Execute(object? parameter)
        {
            _execute(parameter, new RoutedEventArgs());
        }
    }
}