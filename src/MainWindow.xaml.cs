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

        private bool isIoStoreMode = false;
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

        /////////////////////
        // AES KEY SECTION //
        /////////////////////

        // Load AES Key
        private void LoadAesKeys()
        {
            try
            {
                var repakConfig = _configService.LoadRepakConfig<dynamic>();
                string aesKey = repakConfig?.AesKey ?? string.Empty;

                var zentoolsConfig = _configService.LoadZenToolsConfig();

                string zenToolsKeyGuid = zentoolsConfig?.Keys.FirstOrDefault() ?? string.Empty; // Guid stored as a key to ensure it's the first value
                string zenToolsKeyHex = zentoolsConfig?.Values.FirstOrDefault() ?? string.Empty; // Hex stored as a regular value

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



        ///////////////////
        // REPAK SECTION //
        ///////////////////

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

        //////////////////////
        // ZENTOOLS SECTION //
        //////////////////////

        // Start Unpack with ZenTools (.ucas/.utoc)
        private async Task StartZenToolsUnpackAsync()
        {
            var zentoolsConfig = _configService.LoadZenToolsConfig();

            if (zentoolsConfig == null || zentoolsConfig.Count == 0)
            {
                MessageBox.Show("ZenTools AES Key configuration is missing or empty.");
                return;
            }

            string zenToolsKeyGuid = zentoolsConfig.Keys.FirstOrDefault() ?? string.Empty;
            string zenToolsKeyHex = zentoolsConfig.Values.FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(zenToolsKeyGuid) || string.IsNullOrEmpty(zenToolsKeyHex))
            {
                MessageBox.Show("ZenTools AES Key (GUID) not found in the config.\n\nThe GUID cannot be left blank.\n\nDefault GUID: 00000000-0000-0000-0000-000000000000");
                return;
            }
            else
            {
                Debug.WriteLine($"[DEBUG]: ZenTools AES Key Found:\n[DEBUG]: GUID: {zenToolsKeyGuid}\n[DEBUG]: Hex: {zenToolsKeyHex}");
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                MessageBox.Show("Pleaase select an input folder.");
            }

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                MessageBox.Show("Please select an output folder.");
                return;
            }

            string inputPath = inputFolderPath;

            string uniqueGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
            string outputPath = Path.Combine(outputFolderPath, $"PakMaster_IoStore_{uniqueGuid}");



            string encryptionKeysPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs", "zentools-aeskey.json");
            string arguments;

            if (!string.IsNullOrEmpty(zenToolsKeyHex))
            {
                arguments = $"ExtractPackages \"{inputPath}\" \"{outputPath}\" -EncryptionKeys=\"{encryptionKeysPath}\" -ZenPackageVersion=Initial";
            }
            else
            {
                arguments = $"ExtractPackages \"{inputPath}\" \"{outputPath}\" -ZenPackageVersion=Initial";
            }

            await RunToolAsync("zentools", "zentools.exe", arguments, output =>
            {
                UpdateCommandOutput(output);
                RepopulateInputListBox();
                RepopulateOutputListBox();
            });
        }

        /////////////////////////
        // UNREALREZEN SECTION //
        /////////////////////////

        // Start Repack with UnrealReZen (.ucas/.utoc)
        private async Task StartUnrealReZenRepackAsync()
        {
            MessageBox.Show("IoStore file repacking is not supported yet.");
        }

        ////////////////////////////
        // FOLDER BROWSER SECTION //
        ////////////////////////////

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

            if (isIoStoreMode)
            {
                openFileDialog.Filter = "IoStore Files (*.ucas, *.utoc)|*.ucas;*.utoc";
            }
            else
            {
                openFileDialog.Filter = "Pak Files (*.pak)|*.pak";
            }

            if (openFileDialog.ShowDialog() == true)
            {
                inputFolderPath = Path.GetDirectoryName(openFileDialog.FileName);

                if (!string.IsNullOrEmpty(inputFolderPath))
                {
                    List<KeyValuePair<string, string>> files;

                    if (isIoStoreMode)
                    {
                        files = Directory.GetFiles(inputFolderPath, "*.ucas")
                            .Concat(Directory.GetFiles(inputFolderPath, "*.utoc"))
                            .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath))
                            .ToList();
                    }
                    else
                    {
                        files = Directory.GetFiles(inputFolderPath, "*.pak")
                            .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath))
                            .ToList();
                    }

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

        //////////////////////
        // RUN TOOL SECTION //
        //////////////////////

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

        ///////////////////////
        // UI METHOD SECTION //
        ///////////////////////

        // Toggle Switch Event Handler
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (FileTypeToggleSwitch.IsOn)
            {
                // .ucas/.utoc format selected
                isIoStoreMode = true;
                RepopulateInputListBox();
                RepopulateOutputListBox();
            }
            else
            {
                // .pak format selected
                isIoStoreMode = false;
                RepopulateInputListBox();
                RepopulateOutputListBox();
            }
        }

        // Event handlers for unpacking and repacking, checks if toggle is on or off
        private async void btnUnpack_Click(object sender, RoutedEventArgs e)
        {
            if (isIoStoreMode)
            {
                await StartZenToolsUnpackAsync();
            }
            else
            {
                await StartRepakUnpackAsync();
            }
        }

        private async void btnRepack_Click(object sender, RoutedEventArgs e)
        {
            if (isIoStoreMode)
            {
                await StartUnrealReZenRepackAsync();
            }
            else
            {
                await StartRepakRepackAsync();
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
                List<KeyValuePair<string, string>> files = new List<KeyValuePair<string, string>>();

                if (isIoStoreMode)
                {
                    files.AddRange(Directory.GetFiles(inputFolderPath, "*.ucas")
                        .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath)));

                    files.AddRange(Directory.GetFiles(inputFolderPath, "*.utoc")
                        .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath)));
                }
                else
                {
                    files.AddRange(Directory.GetFiles(inputFolderPath, "*.pak")
                        .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath)));
                }

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
