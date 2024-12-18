﻿using MahApps.Metro.Controls;
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
            UrlService.OpenUrl("https://github.com/AriesLR/PakMaster");
        }


        private async void CheckForUpdatesPakMaster(object sender, RoutedEventArgs e)
        {
            await UpdateService.CheckJsonForUpdates("https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/version/update.json?token=GHSAT0AAAAAACY6F73NB7QLGGMNR7HJHCCAZ3CHSYQ"); // Change this after repo goes public
        }

    }
}