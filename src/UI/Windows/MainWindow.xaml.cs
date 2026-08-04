using PakMaster.UI.Taskbar;

namespace PakMaster
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowState();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            TrayIconManager.UpdateTrayIconVisibility();
        }

        // ============ Button Clicks ============

        // Open Github Repo
        private void OpenGithubRepo_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Debug("User clicked Github Repo button.");
            UrlOperations.OpenUrlAsync(AppUrls.GithubRepoUrl);
        }

        // ============ Event Handlers ============

        // OnStateChanged Method
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            var settings = AppSettingsManager.CurrentSettings;

            if (this.WindowState == WindowState.Minimized && settings != null && settings.MinimizeToTray)
            {
                this.Hide();

                GLogger.Here().Information("Application minimized. Intercepted state change and hid application in the Taskbar Tray.");
            }
        }

        // OnClosing Method
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            WindowPositionManager.SavePosition(this);

            if (DataContext is IDisposable disposableVM)
            {
                disposableVM.Dispose();
            }
        }

        // End of Class
    }
}