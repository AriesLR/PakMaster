using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PakMaster.Resources.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isAesKeysFlyoutOpen;

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
