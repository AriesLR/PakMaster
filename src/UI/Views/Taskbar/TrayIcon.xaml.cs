namespace PakMaster.UI.Views.Taskbar
{
    public partial class TrayIcon : UserControl
    {
        public TrayIcon()
        {
            InitializeComponent();
        }

        // Double Click Taskbar Icon Maximize
        private void TaskbarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Debug("User double clicked the taskbar tray icon.");
            MaximizeWindow();
        }

        // Context Menu Maximize
        private void MenuShowApp_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Debug("User double clicked the Show Application button.");
            MaximizeWindow();
        }

        // Context Menu Exit Application
        private void MenuExitApp_Click(object sender, RoutedEventArgs e)
        {
            GLogger.Here().Debug("User double clicked the Exit Application button.");
            Application.Current.Shutdown();
        }

        // Maximize Window
        private static void MaximizeWindow()
        {
            if (Application.Current.MainWindow is Window mainWindow)
            {
                mainWindow.Show();

                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                }
                mainWindow.Activate();
            }
        }
    }
}