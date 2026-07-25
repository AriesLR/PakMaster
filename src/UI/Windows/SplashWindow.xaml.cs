using System.Windows.Media.Imaging;

namespace PakMaster
{
    public partial class SplashWindow : MetroWindow
    {
        public SplashWindow()
        {
            InitializeComponent();
            LoadHighResIcon();
        }

        // Update Loading Message
        public void UpdateStatus(string message)
        {
            Dispatcher.InvokeSafe(() =>
            {
                StatusTextBlock.Text = message;
            });
        }

        public void LoadingComplete()
        {
            Dispatcher.InvokeSafe(() =>
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                StatusTextBlock.Visibility = Visibility.Collapsed;
            });
        }

        // Load Appicon
        private void LoadHighResIcon()
        {
            const string iconUriString = "pack://application:,,,/appicon.ico";

            try
            {
                GLogger.Here().Debug("Attempting to load high-resolution app icon frame from {0}...", iconUriString);

                var iconUri = new Uri(iconUriString);
                var decoder = new IconBitmapDecoder(
                    iconUri,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);

                var bestFrame = decoder.Frames
                    .OrderByDescending(f => f.Width)
                    .ThenByDescending(f => f.Format.BitsPerPixel)
                    .FirstOrDefault();

                if (bestFrame != null)
                {
                    AppIconImage.Source = bestFrame;
                    GLogger.Here().Debug("Successfully loaded high-res icon frame ({0}x{1}, {2}bpp).",
                        bestFrame.PixelWidth, bestFrame.PixelHeight, bestFrame.Format.BitsPerPixel);
                }
                else
                {
                    GLogger.Here().Warning("IconBitmapDecoder parsed the file but returned no valid frames. Falling back to default URI loading.");
                    ApplyFallbackIcon(iconUri);
                }
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to decode high-resolution app icon via IconBitmapDecoder. Attempting fallback.");
                ApplyFallbackIcon(new Uri(iconUriString));
            }
        }

        private void ApplyFallbackIcon(Uri iconUri)
        {
            try
            {
                AppIconImage.Source = new BitmapImage(iconUri);
                GLogger.Here().Information("Successfully applied fallback app icon image.");
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to load fallback app icon. Hiding icon element.");

                AppIconImage.Visibility = Visibility.Collapsed;
            }
        }
    }
}