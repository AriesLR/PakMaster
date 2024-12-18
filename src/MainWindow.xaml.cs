using MahApps.Metro.Controls;
using Microsoft.Win32;
using PakMaster.Resources.Functions.Services;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace PakMaster
{
    public partial class MainWindow : MetroWindow
    {
        private string? inputFolderPath;
        private string? outputFolderPath;


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

        // Start Unpack with Repak (.pak)
        private void StartRepakUnpack(object sender, RoutedEventArgs e)
        {
            // AES Key, eventually add a way to manually enter an AES Key, for now, Stalker 2 support only.
            string aesKey = "0x33A604DF49A07FFD4A4C919962161F5C35A134D37EFA98DB37A34F6450D7D386";

            var selectedInputFile = InputFilesListBox.SelectedItem as KeyValuePair<string, string>?;

            if (selectedInputFile == null)
            {
                MessageBox.Show("Please select a file to unpack.");
                return;
            }

            string fullInputFilePath = selectedInputFile.Value.Value;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullInputFilePath);

            if (string.IsNullOrEmpty(fullInputFilePath))
            {
                MessageBox.Show("Invalid file path.");
                return;
            }

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                MessageBox.Show("Please select an output folder.");
                return;
            }

            string outputPath = Path.Combine(outputFolderPath, fileNameWithoutExtension);

            string arguments = $"-a {aesKey} unpack -o \"{outputPath}\" \"{fullInputFilePath}\"";

            RunTool("repak", "repak.exe", arguments, output =>
            {
                UpdateCommandOutput(output);
            });
        }

        // Start Repack with Repak (.pak)
        private void StartRepakRepack(object sender, RoutedEventArgs e)
        {
            var selectedInputFolder = OutputFilesListBox.SelectedItem as KeyValuePair<string, string>?;

            if (selectedInputFolder == null)
            {
                MessageBox.Show("Please select an input folder to repack.");
                return;
            }

            string fullInputFolderPath = selectedInputFolder.Value.Value;

            if (!Directory.Exists(fullInputFolderPath))
            {
                MessageBox.Show($"The selected folder does not exist: {fullInputFolderPath}");
                return;
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                MessageBox.Show("Please browse and select an input folder first.");
                return;
            }

            string folderName = Path.GetFileName(fullInputFolderPath);

            if (string.IsNullOrEmpty(folderName))
            {
                MessageBox.Show("Invalid input folder name.");
                return;
            }

            string outputPakName = folderName.EndsWith("_P")
                ? folderName.Substring(0, folderName.Length - 2) + "_Modified_P.pak"
                : folderName + "_Modified_P.pak";

            string outputFilePath = Path.Combine(inputFolderPath, outputPakName);

            string arguments = $"pack --version V11 \"{fullInputFolderPath}\" \"{outputFilePath}\"";

            RunTool("repak", "repak.exe", arguments, output =>
            {
                UpdateCommandOutput(output);
            });
        }

        // Browse input folder and populate InputFilesListBox
        private void BrowseInputFolder(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a Folder for Input",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                inputFolderPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(inputFolderPath))
                {
                    List<KeyValuePair<string, string>> files = Directory.GetFiles(inputFolderPath, "*.pak")
                        .Select(filePath => new KeyValuePair<string, string>(
                            Path.GetFileName(filePath),
                            filePath
                        ))
                        .ToList();

                    InputFilesListBox.ItemsSource = files;
                }
            }
        }

        // Browse output folder and populate OutputFilesListBox
        private void BrowseOutputFolder(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a Folder for Output",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                outputFolderPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(outputFolderPath))
                {
                    List<KeyValuePair<string, string>> subdirectories = Directory.GetDirectories(outputFolderPath)
                        .Select(directoryPath => new KeyValuePair<string, string>(
                            Path.GetFileName(directoryPath),
                            directoryPath
                        ))
                        .ToList();

                    OutputFilesListBox.ItemsSource = subdirectories;
                }
            }
        }



        // Run the tool and capture output
        private void RunTool(string toolFolderName, string executableName, string arguments, Action<string> outputCallback)
        {
            try
            {
                // Get the current directory (app location)
                string currentDirectory = Directory.GetCurrentDirectory();

                // Find the tool folder (ZenTools or repak)
                string toolDirectory = Path.Combine(currentDirectory, "bin", toolFolderName);

                // Get the full path to the executable
                string executablePath = Path.Combine(toolDirectory, executableName);

                // Ensure that the tool directory exists
                if (!Directory.Exists(toolDirectory))
                {
                    throw new DirectoryNotFoundException($"Tool directory not found: {toolDirectory}");
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = toolDirectory
                };

                Process process = new Process
                {
                    StartInfo = processStartInfo
                };

                StringBuilder outputBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        outputBuilder.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        outputBuilder.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                outputCallback?.Invoke(outputBuilder.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running command: {ex.Message}");
            }
        }

        private void UpdateCommandOutput(string output)
        {
            // Ensure the UI update is on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommandOutputTextBox.Text = output; // Update the TextBox with captured output
            });
        }

    }
}