using PakMaster.Resources.Functions.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PakMaster.Resources.ViewModels
{

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isAesKeysFlyoutOpen;
        private bool _isIoStoreFlyoutOpen;
        private bool _isSettingsFlyoutOpen;

        public ObservableCollection<MenuItem> MenuItems { get; set; }

        public MainWindowViewModel()
        {
            // Sidebar Items (I hate how I had to do this so much)
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Text = "AES Keys",
                    Icon = "ShieldKey",
                    Command = new RelayCommand((sender, e) => btnSidebarAESKeys_Click(sender, e))
                },
                new MenuItem
                {
                    Text = "Repak Settings",
                    Icon = "AlphaRBox",
                    Command = new RelayCommand((sender, e) => btnSidebarRepakSettings_Click(sender, e))
                },
                new MenuItem
                {
                    Text = "ZenTools Settings",
                    Icon = "AlphaZBox",
                    Command = new RelayCommand((sender, e) => btnSidebarZenToolsSettings_Click(sender, e))
                }
            };
        }

        private void btnSidebarAESKeys_Click(object sender, RoutedEventArgs e)
        {
            OpenAesKeysFlyout();
        }

        private async void btnSidebarRepakSettings_Click(object sender, RoutedEventArgs e)
        {
            await MessageService.ShowInfo("Work In Progress", "This feature is not finished.");
        }

        private async void btnSidebarZenToolsSettings_Click(object sender, RoutedEventArgs e)
        {
            await MessageService.ShowInfo("Work In Progress", "This feature is not finished.");
        }

        // IoStore Packing Flyout
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

        // Toggle IoStore Packing Flyout to Open (true)
        public void OpenIoStoreFlyout()
        {
            if (IsIoStoreFlyoutOpen)
            {
                IsIoStoreFlyoutOpen = false;
            }
            IsIoStoreFlyoutOpen = true;
        }

        // AesKeys Flyout
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

        // Toggle AesKeys Flyout to Open (true)
        public void OpenAesKeysFlyout()
        {
            if (IsAesKeysFlyoutOpen)
            {
                IsAesKeysFlyoutOpen = false;
            }
            IsAesKeysFlyoutOpen = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Sidebar Helper Classes
    public class RelayCommand : ICommand
    {
        private readonly Action<object, RoutedEventArgs> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object, RoutedEventArgs> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? (param => true);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute(parameter);

        public void Execute(object parameter)
        {
            _execute(parameter, null);
        }
    }


    public class MenuItem
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public ICommand Command { get; set; }
    }
}
