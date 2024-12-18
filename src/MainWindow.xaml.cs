using MahApps.Metro.Controls;
using PakMaster.Resources.Functions.Services;
using System.Windows;

namespace PakMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Open PakMaster's GitHub Repo in the user's default browser 
        private void LaunchBrowserGitHubPakMaster(object sender, RoutedEventArgs e)
        {
            UrlService.OpenUrl("https://github.com");
        }


        private async void CheckForUpdatesPakMaster(object sender, RoutedEventArgs e)
        {
            await UpdateService.CheckJsonForUpdates("https://raw.githubusercontent.com/AriesLR/Aetherium/main/docs/version/update.json"); // Change this - temp link from another project
        }

    }
}