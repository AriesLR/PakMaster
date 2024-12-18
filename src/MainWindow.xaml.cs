using MahApps.Metro.Controls;
using Microsoft.Win32;
using PakMaster.Resources.Functions.Services;
using System.IO;
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

        // Check for updates via json
        private async void CheckForUpdatesPakMaster(object sender, RoutedEventArgs e)
        {
            await UpdateService.CheckJsonForUpdates("https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/version/update.json");
        }

        // Browse input folder and populate InputFilesListBox
        private void BrowseInputFolder(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection" // This makes it act like a folder picker
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Get .pak, .ucas, .utoc files in the folder
                    List<string> files = new List<string>();
                    string[] extensions = { "*.pak", "*.ucas", "*.utoc" };

                    foreach (var ext in extensions)
                    {
                        // Add only the file name and extension (no path)
                        var fileNames = Directory.GetFiles(selectedPath, ext)
                            .Select(filePath => Path.GetFileName(filePath)) // Extract file name and extension
                            .ToList();

                        files.AddRange(fileNames);
                    }

                    // Display files (file names + extensions) in the InputFilesListBox
                    InputFilesListBox.ItemsSource = files;
                }
            }
        }

        // Browse output folder
        private void BrowseOutputFolder(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a Folder for Output",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection" // This allows the user to pick a folder
            };

            // Show the dialog and check if the user selected a folder
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Get only subdirectories within the selected folder
                    string[] subdirectories = Directory.GetDirectories(selectedPath)
                        .Select(directoryPath => Path.GetFileName(directoryPath)) // Extract only folder names
                        .ToArray();

                    // Display folder names in the OutputFilesListBox
                    OutputFilesListBox.ItemsSource = subdirectories;
                }
            }
        }
    }
}