using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PakMaster.Resources.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isAesKeysFlyoutOpen;
        private bool _isIoStoreFlyoutOpen;
        private bool _isSettingsFlyoutOpen;

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
            IsAesKeysFlyoutOpen = true;
        }

        // Settings Flyout
        public bool IsSettingsFlyoutOpen
        {
            get => _isSettingsFlyoutOpen;
            set
            {
                if (_isSettingsFlyoutOpen != value)
                {
                    _isSettingsFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        // Toggle Settings Flyout to Open (true)
        public void OpenSettingsFlyout()
        {
            IsSettingsFlyoutOpen = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
