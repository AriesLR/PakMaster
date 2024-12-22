using MahApps.Metro.Controls;
using Microsoft.Win32;
using PakMaster.Resources.Functions.Services;
using PakMaster.Resources.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace PakMaster
{
    public partial class MainWindow : MetroWindow
    {
        private ConfigService _configService;

        private bool isZenToolsFormat = false;
        private string? inputFolderPath;
        private string? outputFolderPath;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            _configService = new ConfigService();
            LoadAesKeys();
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

        // Load AES Key
        private void LoadAesKeys()
        {
            try
            {
                // Load the Repak config (assuming it's a simple config)
                var repakConfig = _configService.LoadRepakConfig<dynamic>();
                string aesKey = repakConfig?.AesKey ?? string.Empty;

                // Load the ZenTools config
                var zentoolsConfig = _configService.LoadZenToolsConfig<Dictionary<string, string>>();

                // Extract the guid and aesKey from the ZenTools config
                string zenToolsKeyGuid = zentoolsConfig?.Keys.FirstOrDefault() ?? string.Empty; // Guid stored as a key to ensure it's the first value
                string zenToolsKeyHex = zentoolsConfig?.Values.FirstOrDefault() ?? string.Empty; // Hex stored as a regular value

                // Update the UI
                AesKeyTextBox.Text = aesKey;
                ZenToolsKeyGuidTextBox.Text = zenToolsKeyGuid;
                ZenToolsKeyHexTextBox.Text = zenToolsKeyHex;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // Save Repak AES Key
        private void SaveRepakConfig(object sender, RoutedEventArgs e)
        {
            string aesKey = AesKeyTextBox.Text.Trim();

            var config = new { AesKey = aesKey };

            _configService.SaveRepakConfig(config);

            MessageBox.Show("Repak configuration saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Save ZenTools AES Key
        private void SaveZenToolsConfig(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the AES key and GUID from the UI text boxes
                string zenToolsKeyGuid = ZenToolsKeyGuidTextBox.Text.Trim();
                string zenToolsKeyHex = ZenToolsKeyHexTextBox.Text.Trim();

                // Save the config to file (pass guid and aesKey separately)
                _configService.SaveZenToolsConfig(zenToolsKeyGuid, zenToolsKeyHex);

                MessageBox.Show("ZenTools configuration saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving ZenTools config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        // Start Unpack with Repak (.pak)
        private async Task StartRepakUnpackAsync()
        {
            // Load the AES Key from the config
            var config = _configService.LoadRepakConfig<dynamic>();
            string aesKey = config?.AesKey ?? string.Empty;

            if (string.IsNullOrEmpty(aesKey))
            {
                Debug.WriteLine("[DEBUG]: AES Key is empty");
            }
            else
            {
                Debug.WriteLine($"[DEBUG]: AES Key found.\n[DEBUG]: AES Key: {aesKey}");
            }

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

            string arguments = string.IsNullOrEmpty(aesKey)
                ? $"unpack -o \"{outputPath}\" \"{fullInputFilePath}\""
                : $"-a {aesKey} unpack -o \"{outputPath}\" \"{fullInputFilePath}\"";

            await RunToolAsync("repak", "repak.exe", arguments, output =>
            {
                UpdateCommandOutput(output);
                RepopulateInputListBox();
                RepopulateOutputListBox();
            });
        }


        // Start Repack with Repak (.pak)
        private async Task StartRepakRepackAsync()
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

            await RunToolAsync("repak", "repak.exe", arguments, output =>
            {
                UpdateCommandOutput(output);
                RepopulateInputListBox();
                RepopulateOutputListBox();
            });
        }

        // Start Unpack with ZenTools (.ucas/.utoc)
        private async Task StartZenToolsUnpackAsync()
        {
            MessageBox.Show("ZenTools unpacking is not yet supported.");
        }

        // Start Repack with ZenTools (.ucas/.utoc)
        private async Task StartZenToolsRepackAsync()
        {
            MessageBox.Show("ZenTools repacking is not yet supported.");
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

            // Use the last selected input folder path if available
            if (!string.IsNullOrEmpty(inputFolderPath))
            {
                openFileDialog.InitialDirectory = inputFolderPath;
            }

            if (openFileDialog.ShowDialog() == true)
            {
                inputFolderPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(inputFolderPath))
                {
                    List<KeyValuePair<string, string>> files = Directory.GetFiles(inputFolderPath, "*.pak")
                        .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath))
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

            // Use the last selected output folder path if available
            if (!string.IsNullOrEmpty(outputFolderPath))
            {
                openFileDialog.InitialDirectory = outputFolderPath;
            }

            if (openFileDialog.ShowDialog() == true)
            {
                outputFolderPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(outputFolderPath))
                {
                    List<KeyValuePair<string, string>> subdirectories = Directory.GetDirectories(outputFolderPath)
                        .Select(directoryPath => new KeyValuePair<string, string>(Path.GetFileName(directoryPath), directoryPath))
                        .ToList();

                    OutputFilesListBox.ItemsSource = subdirectories;
                }
            }
        }

        // Run the tool and capture output asynchronously
        private async Task RunToolAsync(string toolFolderName, string executableName, string arguments, Action<string> outputCallback)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string toolDirectory = Path.Combine(currentDirectory, "bin", toolFolderName);
                string executablePath = Path.Combine(toolDirectory, executableName);

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

                Process process = new Process { StartInfo = processStartInfo };

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

                await Task.Run(() => process.WaitForExit());

                outputCallback?.Invoke(outputBuilder.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running command: {ex.Message}");
            }
        }

        // Toggle Switch Event Handler
        /*
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (FileTypeToggleSwitch.IsOn)
            {
                // .ucas/.utoc format selected
                isZenToolsFormat = true;
            }
            else
            {
                // .pak format selected
                isZenToolsFormat = false;
            }
        }
        */

        // Event handlers for unpacking and repacking, checks if toggle is on or off
        private async void btnRepack_Click(object sender, RoutedEventArgs e)
        {
            if (isZenToolsFormat)
            {
                await StartZenToolsRepackAsync();
            }
            else
            {
                await StartRepakRepackAsync();
            }
        }

        private async void btnUnpack_Click(object sender, RoutedEventArgs e)
        {
            if (isZenToolsFormat)
            {
                await StartZenToolsUnpackAsync();
            }
            else
            {
                await StartRepakUnpackAsync();
            }
        }


        private void UpdateCommandOutput(string output)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommandOutputTextBox.Text += output + Environment.NewLine;
            });
        }

        // Methods for repopulating listboxes after unpacking or repacking
        private void RepopulateInputListBox()
        {
            if (!string.IsNullOrEmpty(inputFolderPath))
            {
                List<KeyValuePair<string, string>> files = Directory.GetFiles(inputFolderPath, "*.pak")
                    .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath))
                    .ToList();

                InputFilesListBox.ItemsSource = files;
            }
        }

        private void RepopulateOutputListBox()
        {
            if (!string.IsNullOrEmpty(outputFolderPath))
            {
                List<KeyValuePair<string, string>> subdirectories = Directory.GetDirectories(outputFolderPath)
                    .Select(directoryPath => new KeyValuePair<string, string>(Path.GetFileName(directoryPath), directoryPath))
                    .ToList();

                OutputFilesListBox.ItemsSource = subdirectories;
            }
        }

        // Open AesKeys Flyout (Settings/Config)
        private void OpenAesKeysFlyout(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.OpenAesKeysFlyout();
                LoadAesKeys(); // Load again here in case user changes the values via the config directly.
            }
        }
    }
}
