namespace PakMaster.Core.Models
{
    public class SidebarModel : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private Func<string>? _textGetter;

        public Func<string>? TextGetter
        {
            get => _textGetter;
            set
            {
                _textGetter = value;
                UpdateText();
            }
        }

        public string Text
        {
            get => _textGetter != null ? _textGetter() : _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Icon { get; set; } = string.Empty;
        public ICommand Command { get; set; } = new RelayCommand((s, e) => { });

        public void UpdateText()
        {
            OnPropertyChanged(nameof(Text));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}