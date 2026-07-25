namespace PakMaster.UI.Views
{
    public partial class MainView : UserControl
    {
        private bool _isInitialized = false;
        private bool isIoStoreMode = false;
        private string? inputFolderPath;
        private string? outputFolderPath;

        public MainView()
        {
            InitializeComponent();
            _isInitialized = true;
            DataContext = new MainWindowState();
        }

        // Open PakMaster's GitHub Repo in the user's default browser
        private void LaunchBrowserGitHubPakMaster(object sender, RoutedEventArgs e)
        {
            UrlOperations.OpenUrlAsync(AppUrls.GithubRepoUrl);
        }

        // Check for updates via json
        private async void CheckForUpdatesPakMasterAsync(object sender, RoutedEventArgs e)
        {
            await UpdateManager.CheckForUpdatesAsync(AppUrls.UpdateUrl);
        }

        /////////////////////
        // AES KEY SECTION //
        /////////////////////

        ///////////////////
        // REPAK SECTION //
        ///////////////////

        // Start Unpack with Repak (.pak)
        private async Task StartRepakUnpackAsync()
        {
            // Load the AES Key from the config
            var config = ConfigManager.CurrentSettings;
            string aesKey = config.Repak.AesKey;

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
                await MessageManager.ShowWarning("Please select a file to unpack.");
                return;
            }

            string fullInputFilePath = selectedInputFile.Value.Value;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullInputFilePath);

            if (string.IsNullOrEmpty(fullInputFilePath))
            {
                await MessageManager.ShowWarning("Invalid file path.");
                return;
            }

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                await MessageManager.ShowWarning("Please select an output folder.");
                return;
            }

            string outputPath = Path.Combine(outputFolderPath, fileNameWithoutExtension);

            string arguments = string.IsNullOrEmpty(aesKey)
                ? $"unpack -o \"{outputPath}\" \"{fullInputFilePath}\""
                : $"-a {aesKey} unpack -o \"{outputPath}\" \"{fullInputFilePath}\"";

            await RunToolAsync("repak", "repak.exe", arguments, output =>
            {
                UpdateCommandOutput(output);
                RefreshUI();
            });
        }

        // Start Repack with Repak (.pak)
        private async Task StartRepakRepackAsync()
        {
            var selectedInputFolder = OutputFilesListBox.SelectedItem as KeyValuePair<string, string>?;

            var repakConfig = ConfigManager.CurrentSettings.Repak;
            string repakVersion = repakConfig.RepakVersion;

            if (selectedInputFolder == null)
            {
                await MessageManager.ShowWarning("Please select an input folder to repack.");
                return;
            }

            string fullInputFolderPath = selectedInputFolder.Value.Value;

            if (!Directory.Exists(fullInputFolderPath))
            {
                await MessageManager.ShowWarning($"The selected folder does not exist: {fullInputFolderPath}");
                return;
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                await MessageManager.ShowWarning("Please browse and select an input folder first.");
                return;
            }

            string folderName = Path.GetFileName(fullInputFolderPath);

            if (string.IsNullOrEmpty(folderName))
            {
                await MessageManager.ShowWarning("Invalid input folder name.");
                return;
            }

            string outputPakName = folderName.EndsWith("_P")
                ? folderName.Substring(0, folderName.Length - 2) + "_Modified_P.pak"
                : folderName + "_Modified_P.pak";

            string outputFilePath = Path.Combine(inputFolderPath, outputPakName);

            string arguments = $"pack --version {repakVersion} \"{fullInputFolderPath}\" \"{outputFilePath}\"";

            await RunToolAsync("repak", "repak.exe", arguments, output =>
            {
                UpdateCommandOutput(output);
                RefreshUI();
            });
        }

        //////////////////////
        // ZENTOOLS SECTION //
        //////////////////////

        // Start Unpack with ZenTools (.ucas/.utoc)
        private async Task StartZenToolsUnpackAsync()
        {
            var zentoolsConfig = ConfigManager.LoadZenToolsConfig();
            string zenToolsKeyGuid = string.Empty;
            string zenToolsKeyHex = string.Empty;
            if (zentoolsConfig != null)
            {
                foreach (var kvp in zentoolsConfig)
                {
                    zenToolsKeyGuid = kvp.Key;
                    zenToolsKeyHex = kvp.Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(zenToolsKeyGuid))
            {
                await MessageManager.ShowError("ZenTools AES Key (GUID) not found in the config.\n\nThe GUID cannot be left blank.\n\nDefault GUID: 00000000-0000-0000-0000-000000000000");
                return;
            }

            if (string.IsNullOrEmpty(zenToolsKeyHex))
            {
                Debug.WriteLine($"[DEBUG]: No ZenTools AES Key Hex Found.");
            }
            else
            {
                Debug.WriteLine($"[DEBUG]: ZenTools AES Key Found:\n[DEBUG]: GUID: {zenToolsKeyGuid}\n[DEBUG]: Hex: {zenToolsKeyHex}");
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                await MessageManager.ShowWarning("Please select an input folder.");
                return;
            }

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                await MessageManager.ShowWarning("Please select an output folder.");
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
                RefreshUI();
            });

            string appDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar));
            string engineFolderPath = Path.Combine(appDirectory, "Engine");
            string zenToolsFolderPath = Path.Combine(appDirectory, "ZenTools");

            try
            {
                if (Directory.Exists(engineFolderPath))
                {
                    Directory.Delete(engineFolderPath, true);
                    Debug.WriteLine($"[DEBUG]: Deleted folder: {engineFolderPath}");
                }

                if (Directory.Exists(zenToolsFolderPath))
                {
                    Directory.Delete(zenToolsFolderPath, true);
                    Debug.WriteLine($"[DEBUG]: Deleted folder: {zenToolsFolderPath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR]: Failed to delete folders: {ex.Message}");
            }
        }

        ///////////////////////
        // UNREALPAK SECTION //
        ///////////////////////

        // Start Packing with UnrealPak
        private async Task StartUnrealPakRepackAsync()
        {
            var unrealPakConfig = ConfigManager.CurrentSettings.UnrealPak;
            string unrealPakPath = unrealPakConfig.UnrealPakPath;
            string globalOutputPath = unrealPakConfig.GlobalOutputPath;
            string cookedFilesPath = unrealPakConfig.CookedFilesPath;
            string packageStorePath = unrealPakConfig.PackageStorePath;
            string scriptObjectsPath = unrealPakConfig.ScriptObjectsPath;
            string ioStoreCommandsPath = unrealPakConfig.IoStoreCommandsPath;

            if (string.IsNullOrEmpty(unrealPakPath) || !File.Exists(unrealPakPath))
            {
                await MessageManager.ShowWarning("UnrealPak executable path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(globalOutputPath))
            {
                await MessageManager.ShowWarning("Please specify an output path.");
                return;
            }

            if (string.IsNullOrEmpty(cookedFilesPath) || !Directory.Exists(cookedFilesPath))
            {
                await MessageManager.ShowWarning("Cooked files path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(packageStorePath) || !File.Exists(packageStorePath))
            {
                await MessageManager.ShowWarning("PackageStore.manifest path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(scriptObjectsPath) || !File.Exists(scriptObjectsPath))
            {
                await MessageManager.ShowWarning("ScriptObjects.bin path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(ioStoreCommandsPath) || !File.Exists(ioStoreCommandsPath))
            {
                await MessageManager.ShowWarning("IoStoreCommands.txt path is missing or invalid.");
                return;
            }

            string cryptoKeysPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs", "Crypto.json");

            string finalGlobalOutputPath = Path.Combine(globalOutputPath, "global.utoc");

            string arguments = $"-CreateGlobalContainer=\"{finalGlobalOutputPath}\" " +
                               $"-CookedDirectory=\"{cookedFilesPath}\" " +
                               $"-WriteBackMetadataToAssetRegistry=Disabled " +
                               $"-PackageStoreManifest=\"{packageStorePath}\" " +
                               $"-Commands=\"{ioStoreCommandsPath}\" " +
                               $"-ScriptObjects=\"{scriptObjectsPath}\" " +
                               $"-patchpaddingalign=2048 " +
                               $"-compressionformats=Oodle " +
                               $"-compresslevel=4 " +
                               $"-compressionmethod=Kraken " +
                               $"-cryptokeys=\"{cryptoKeysPath}\" " +
                               $"-compressionMinBytesSaved=1024 " +
                               $"-compressionMinPercentSaved=5";

            Debug.WriteLine($"[DEBUG]: UnrealPak Configuration Loaded:");
            Debug.WriteLine($"[DEBUG]: UnrealPak Path: {unrealPakPath}");
            Debug.WriteLine($"[DEBUG]: Output Path: {finalGlobalOutputPath}");
            Debug.WriteLine($"[DEBUG]: Cooked Files Path: {cookedFilesPath}");
            Debug.WriteLine($"[DEBUG]: PackageStore Path: {packageStorePath}");
            Debug.WriteLine($"[DEBUG]: IoStoreCommands Path: {ioStoreCommandsPath}");
            Debug.WriteLine($"[DEBUG]: ScriptObjects Path: {scriptObjectsPath}");
            Debug.WriteLine($"[DEBUG]: Crypto Keys Path: {cryptoKeysPath}");
            Debug.WriteLine($"[DEBUG]: Arguments: {arguments}");

            await RunUnrealPakAsync(unrealPakPath, arguments, output =>
            {
                UpdateCommandOutput(output);
                RefreshUI();
            });
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
                openFileDialog.Filter = "IoStore Files (*.pak, *.ucas, *.utoc)|*.pak;*.ucas;*.utoc";
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
                        files = Directory.GetFiles(inputFolderPath, "*.pak")
                            .Concat(Directory.GetFiles(inputFolderPath, "*.ucas"))
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



        ///////////////////////////
        // START PROCESS SECTION //
        ///////////////////////////

        // Run the proper tool and capture output
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
                await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        // Run UnrealPak helper
        private async Task RunUnrealPakAsync(string unrealPakPath, string arguments, Action<string> outputCallback)
        {
            try
            {
                if (string.IsNullOrEmpty(unrealPakPath))
                {
                    throw new ArgumentException("UnrealPak path is not provided.");
                }

                if (!File.Exists(unrealPakPath))
                {
                    throw new FileNotFoundException($"UnrealPak executable not found: {unrealPakPath}");
                }

                string workingDirectory = Path.GetDirectoryName(unrealPakPath);
                if (string.IsNullOrEmpty(workingDirectory))
                {
                    throw new DirectoryNotFoundException("Could not determine the working directory for UnrealPak.");
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = unrealPakPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
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
                await MessageManager.ShowError($"Error running command: {ex.Message}");
            }
        }

        // Open Crypto.json in user's default app for .json files

        ////////////////////////
        // UI ELEMENT SECTION //
        ////////////////////////

        // Refresh Button
        private void btnRefreshUI_Click(object sender, RoutedEventArgs e)
        {
            RepopulateInputListBox();
            RepopulateOutputListBox();
        }

        // Mode Switch Button - Normal
        private void btnModeSwitchNormal_Click(object sender, RoutedEventArgs e)
        {
            ModeSwitchButton.Content = "Normal Mode";
            isIoStoreMode = false; // Normal Mode (.pak)
            RefreshUI();
        }

        // Mode Switch Button - IoStore
        private void btnModeSwitchIoStore_Click(object sender, RoutedEventArgs e)
        {
            ModeSwitchButton.Content = "IoStore Mode";
            isIoStoreMode = true; // IoStore Mode (.ucas/.utoc)
            RefreshUI();
        }

        // Unpack Button
        private async void btnUnpack_ClickAsync(object sender, RoutedEventArgs e)
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

        // Repack Button
        private async void btnRepack_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (isIoStoreMode)
            {
                if (System.Windows.Application.Current.MainWindow.DataContext is MainWindowState state)
                {
                    state.OpenIoStoreFlyout();
                }
            }
            else
            {
                await StartRepakRepackAsync();
            }
        }

        // IoStore Package Button
        private async void btnIoStorePackage_ClickAsync(object sender, RoutedEventArgs e)
        {
            await StartUnrealPakRepackAsync();
        }

        // Open AesKeys Flyout (Settings/Config)
        private void OpenAesKeysFlyout(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowState viewModel)
            {
                viewModel.OpenAesKeysFlyout();
                // Load again here in case user changes the values via the config directly.
            }
        }

        ///////////////////////
        // UI METHOD SECTION //
        ///////////////////////

        // Refresh UI Elements
        private void RefreshUI()
        {
            RepopulateInputListBox();
            RepopulateOutputListBox();
        }

        // Update CLI Output
        private void UpdateCommandOutput(string output)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommandOutputTextBox.Text += output + Environment.NewLine; // Main page output

            });
        }

        // Scroll CLI Outputs
        private void CliOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommandOutputTextBox.ScrollToEnd();

        }

        // Repopulate Input ListBox
        private void RepopulateInputListBox()
        {
            if (!string.IsNullOrEmpty(inputFolderPath))
            {
                List<KeyValuePair<string, string>> files = new List<KeyValuePair<string, string>>();

                if (isIoStoreMode)
                {
                    files.AddRange(Directory.GetFiles(inputFolderPath, "*.pak")
                        .Select(filePath => new KeyValuePair<string, string>(Path.GetFileName(filePath), filePath)));

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

        // Repopulate Output ListBox
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
    }
}