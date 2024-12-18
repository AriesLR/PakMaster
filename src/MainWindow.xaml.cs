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
            // You can either use a fixed aesKey or retrieve it from somewhere else
            string aesKey = "0x33A604DF49A07FFD4A4C919962161F5C35A134D37EFA98DB37A34F6450D7D386";  // Update this as needed

            // Get the selected input file from InputFilesListBox (which contains both display names and full paths)
            var selectedInputFile = InputFilesListBox.SelectedItem as KeyValuePair<string, string>?;

            if (selectedInputFile == null)
            {
                MessageBox.Show("Please select a file to unpack.");
                return;
            }

            string fullInputFilePath = selectedInputFile.Value.Value; // The full path is stored as the Value of the KeyValuePair
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullInputFilePath);

            if (string.IsNullOrEmpty(fullInputFilePath))
            {
                MessageBox.Show("Invalid file path.");
                return;
            }

            // Check if the output folder path is available
            if (string.IsNullOrEmpty(outputFolderPath))
            {
                MessageBox.Show("Please select an output folder.");
                return;
            }

            string outputPath = Path.Combine(outputFolderPath, fileNameWithoutExtension);

            // Construct the arguments for the unpack command
            string arguments = $"-a {aesKey} unpack -o \"{outputPath}\" \"{fullInputFilePath}\"";

            // Run the repak tool with the unpack arguments
            RunTool("repak", "repak.exe", arguments, output =>
            {
                // Update the UI with the output (in a TextBox, for example)
                UpdateCommandOutput(output);
            });
        }



        // Start Repack with Repak (.pak)
        private void StartRepakRepack()
        {
            // Get the selected input folder from OutputFilesListBox (folder to be repacked)
            string selectedInputFolder = OutputFilesListBox.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedInputFolder))
            {
                MessageBox.Show("Please select an input folder to repack.");
                return;
            }

            // Ensure the selected input folder exists
            string inputFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "repak_cli", selectedInputFolder);

            if (!Directory.Exists(inputFolderPath))
            {
                MessageBox.Show($"The selected folder does not exist: {inputFolderPath}");
                return;
            }

            // Get the output path (where to save the .pak) from InputFilesListBox
            string selectedOutputFolder = Path.GetDirectoryName(InputFilesListBox.SelectedItem as string);

            if (string.IsNullOrEmpty(selectedOutputFolder))
            {
                MessageBox.Show("Please select an output folder.");
                return;
            }

            // Ensure the output folder exists
            if (!Directory.Exists(selectedOutputFolder))
            {
                MessageBox.Show($"The selected output folder does not exist: {selectedOutputFolder}");
                return;
            }

            // Construct the output file path for the .pak (saved in the output folder)
            string outputFilePath = Path.Combine(selectedOutputFolder, "repacked.pak");

            // Construct the arguments for the repack command
            string arguments = $"pack --version V11 \"{inputFolderPath}\" \"{outputFilePath}\"";

            // Run the repak tool with the repack arguments
            RunTool("repak", "repak.exe", arguments, output =>
            {
                // Update the UI with the output (e.g., in a TextBox)
                UpdateCommandOutput(output);
            });
        }



        // Browse input folder and populate InputFilesListBox
        private void BrowseInputFolder(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    List<KeyValuePair<string, string>> files = new List<KeyValuePair<string, string>>();
                    string[] extensions = { "*.pak", "*.ucas", "*.utoc" };

                    foreach (var ext in extensions)
                    {
                        var fileNames = Directory.GetFiles(selectedPath, ext)
                            .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath))  // Store both file name and full path
                            .ToList();
                        files.AddRange(fileNames);
                    }
                    // Set the ListBox's ItemsSource to the list of files (only displaying the file names)
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
                    // Populate OutputFilesListBox with subdirectories (if necessary)
                    string[] subdirectories = Directory.GetDirectories(outputFolderPath)
                        .Select(directoryPath => Path.GetFileName(directoryPath))
                        .ToArray();
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

                // Set up the process start info
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

        // Update the output text box with the command result
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