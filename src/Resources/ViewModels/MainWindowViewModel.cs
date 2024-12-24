using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PakMaster.Resources.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isAesKeysFlyoutOpen;
        private bool _isIoStoreFlyoutOpen;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
