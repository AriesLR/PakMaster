using MahApps.Metro.Controls;
using Microsoft.Win32;
using PakMaster.Resources.Functions.Services;
using PakMaster.Resources.ViewModels;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PakMaster
{
    public partial class MainWindow : MetroWindow
    {
        private ConfigService _configService;

        private bool _isInitialized = false;
        private bool isIoStoreMode = false;
        private string? inputFolderPath;
        private string? outputFolderPath;

        public MainWindow()
        {
            InitializeComponent();
            _isInitialized = true;
            DataContext = new MainWindowViewModel();
            _configService = new ConfigService();

            LoadRepakVersionInfo();

            // Load Configs
            LoadAesKeysAsync();
            LoadGeneralConfigAsync();
            LoadUnrealPakConfigAsync();
        }

        // Open PakMaster's GitHub Repo in the user's default browser 
        private void LaunchBrowserGitHubPakMaster(object sender, RoutedEventArgs e)
        {
            UrlService.OpenUrlAsync("https://github.com/AriesLR/PakMaster");
        }

        // Check for updates via json
        private async void CheckForUpdatesPakMasterAsync(object sender, RoutedEventArgs e)
        {
            await UpdateService.CheckJsonForUpdatesAsync("https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/version/update.json");
        }

        // Load General Config
        private async void LoadGeneralConfigAsync()
        {
            try
            {
                var generalConfig = _configService.LoadGeneralConfig<dynamic>();

                string repakVersion = generalConfig?.ContainsKey("RepakVersion") ?? false ? generalConfig["RepakVersion"] : string.Empty;

                if (!string.IsNullOrEmpty(repakVersion))
                {
                    if (repakVersion == "V11")
                        RepakVersionSwitchDropdown.SelectedIndex = 0;
                    else if (repakVersion == "V10")
                        RepakVersionSwitchDropdown.SelectedIndex = 1;
                    else if (repakVersion == "V9")
                        RepakVersionSwitchDropdown.SelectedIndex = 2;
                    else if (repakVersion == "V8B")
                        RepakVersionSwitchDropdown.SelectedIndex = 3;
                    else if (repakVersion == "V8A")
                        RepakVersionSwitchDropdown.SelectedIndex = 4;
                    else if (repakVersion == "V7")
                        RepakVersionSwitchDropdown.SelectedIndex = 5;
                    else if (repakVersion == "V6")
                        RepakVersionSwitchDropdown.SelectedIndex = 6;
                    else if (repakVersion == "V5")
                        RepakVersionSwitchDropdown.SelectedIndex = 7;
                    else if (repakVersion == "V4")
                        RepakVersionSwitchDropdown.SelectedIndex = 8;
                    else if (repakVersion == "V3")
                        RepakVersionSwitchDropdown.SelectedIndex = 9;
                    else if (repakVersion == "V2")
                        RepakVersionSwitchDropdown.SelectedIndex = 10;
                    else if (repakVersion == "V1")
                        RepakVersionSwitchDropdown.SelectedIndex = 11;
                }
                Debug.WriteLine($"[DEBUG]: General Config Loaded\n[DEBUG]: RepakVersion: {repakVersion}");
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Error loading General config: {ex.Message}");
            }
        }

        private async void SaveGeneralConfigAsync(string version)
        {
            try
            {
                var config = _configService.LoadGeneralConfig<dynamic>() ?? new ExpandoObject();
                config.RepakVersion = version;
                _configService.SaveGeneralConfig(config);
                Debug.WriteLine($"[DEBUG]: Repak Version Set To {version}");
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Error saving General config: {ex.Message}");
            }
        }

        // Load UnrealPak Config
        private async void LoadUnrealPakConfigAsync()
        {
            try
            {
                var unrealPakConfig = _configService.LoadUnrealPakConfig<Dictionary<string, string>>();

                string unrealPakPath = unrealPakConfig?.ContainsKey("UnrealPakPath") ?? false ? unrealPakConfig["UnrealPakPath"] : string.Empty;
                string globalOutputPath = unrealPakConfig?.ContainsKey("GlobalOutputPath") ?? false ? unrealPakConfig["GlobalOutputPath"] : string.Empty;
                string cookedFilesPath = unrealPakConfig?.ContainsKey("CookedFilesPath") ?? false ? unrealPakConfig["CookedFilesPath"] : string.Empty;
                string packageStorePath = unrealPakConfig?.ContainsKey("PackageStorePath") ?? false ? unrealPakConfig["PackageStorePath"] : string.Empty;
                string scriptObjectsPath = unrealPakConfig?.ContainsKey("ScriptObjectsPath") ?? false ? unrealPakConfig["ScriptObjectsPath"] : string.Empty;
                string ioStoreCommandsPath = unrealPakConfig?.ContainsKey("IoStoreCommandsPath") ?? false ? unrealPakConfig["IoStoreCommandsPath"] : string.Empty;

                UnrealPakPathTextBox.Text = unrealPakPath;
                GlobalOutputPathTextBox.Text = globalOutputPath;
                CookedFilesPathTextBox.Text = cookedFilesPath;
                PackageStorePathTextBox.Text = packageStorePath;
                ScriptObjectsPathTextBox.Text = scriptObjectsPath;
                IoStoreCommandsPathTextBox.Text = ioStoreCommandsPath;
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Error loading UnrealPak config: {ex.Message}");
            }
        }

        /////////////////////
        // AES KEY SECTION //
        /////////////////////

        // Load AES Key
        private async void LoadAesKeysAsync()
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
                await MessageService.ShowError($"Error loading config: {ex.Message}");
            }
        }

        // Save Repak AES Key
        private async void SaveRepakConfigAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                string aesKey = AesKeyTextBox.Text.Trim();

                var config = new { AesKey = aesKey };

                _configService.SaveRepakConfig(config);

                await MessageService.ShowInfo("Success", "Repak configuration saved successfully!");
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Error saving Repak AES Keys config: {ex.Message}");
            }
        }

        // Save ZenTools AES Key
        private async void SaveZenToolsConfigAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                string zenToolsKeyGuid = ZenToolsKeyGuidTextBox.Text.Trim();
                string zenToolsKeyHex = ZenToolsKeyHexTextBox.Text.Trim();

                _configService.SaveZenToolsConfig(zenToolsKeyGuid, zenToolsKeyHex);

                await MessageService.ShowInfo("Success", "ZenTools configuration saved successfully!");
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Error saving ZenTools AES Keys config: {ex.Message}");
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
                await MessageService.ShowWarning("Please select a file to unpack.");
                return;
            }

            string fullInputFilePath = selectedInputFile.Value.Value;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullInputFilePath);

            if (string.IsNullOrEmpty(fullInputFilePath))
            {
                await MessageService.ShowWarning("Invalid file path.");
                return;
            }

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                await MessageService.ShowWarning("Please select an output folder.");
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

            var generalConfig = _configService.LoadGeneralConfig<dynamic>();

            string repakVersion = generalConfig?.ContainsKey("RepakVersion") ?? false ? generalConfig["RepakVersion"] : string.Empty;

            if (selectedInputFolder == null)
            {
                await MessageService.ShowWarning("Please select an input folder to repack.");
                return;
            }

            string fullInputFolderPath = selectedInputFolder.Value.Value;

            if (!Directory.Exists(fullInputFolderPath))
            {
                await MessageService.ShowWarning($"The selected folder does not exist: {fullInputFolderPath}");
                return;
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                await MessageService.ShowWarning("Please browse and select an input folder first.");
                return;
            }

            string folderName = Path.GetFileName(fullInputFolderPath);

            if (string.IsNullOrEmpty(folderName))
            {
                await MessageService.ShowWarning("Invalid input folder name.");
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
            var zentoolsConfig = _configService.LoadZenToolsConfig();

            if (zentoolsConfig == null || zentoolsConfig.Count == 0)
            {
                await MessageService.ShowError("ZenTools AES Key configuration is missing or empty.");
                return;
            }

            string zenToolsKeyGuid = zentoolsConfig.Keys.FirstOrDefault() ?? string.Empty;
            string zenToolsKeyHex = zentoolsConfig.Values.FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(zenToolsKeyGuid))
            {
                await MessageService.ShowError("ZenTools AES Key (GUID) not found in the config.\n\nThe GUID cannot be left blank.\n\nDefault GUID: 00000000-0000-0000-0000-000000000000");
                return;
            }
            else
            {
                Debug.WriteLine($"[DEBUG]: No ZenTools AES Key Found.");
            }

            if (!string.IsNullOrEmpty(zenToolsKeyHex))
            {
                Debug.WriteLine($"[DEBUG]: ZenTools AES Key Found:\n[DEBUG]: GUID: {zenToolsKeyGuid}\n[DEBUG]: Hex: {zenToolsKeyHex}");
            }

            if (string.IsNullOrEmpty(inputFolderPath))
            {
                await MessageService.ShowWarning("Please select an input folder.");
            }

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                await MessageService.ShowWarning("Please select an output folder.");
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
            var unrealPakConfig = _configService.LoadUnrealPakConfig<Dictionary<string, string>>();

            if (unrealPakConfig == null || unrealPakConfig.Count == 0)
            {
                await MessageService.ShowError("UnrealPak configuration is missing or empty.");
                return;
            }

            string unrealPakPath = unrealPakConfig.ContainsKey("UnrealPakPath") ? unrealPakConfig["UnrealPakPath"] : string.Empty;
            string globalOutputPath = unrealPakConfig.ContainsKey("GlobalOutputPath") ? unrealPakConfig["GlobalOutputPath"] : string.Empty;
            string cookedFilesPath = unrealPakConfig.ContainsKey("CookedFilesPath") ? unrealPakConfig["CookedFilesPath"] : string.Empty;
            string packageStorePath = unrealPakConfig.ContainsKey("PackageStorePath") ? unrealPakConfig["PackageStorePath"] : string.Empty;
            string scriptObjectsPath = unrealPakConfig.ContainsKey("ScriptObjectsPath") ? unrealPakConfig["ScriptObjectsPath"] : string.Empty;
            string ioStoreCommandsPath = unrealPakConfig.ContainsKey("IoStoreCommandsPath") ? unrealPakConfig["IoStoreCommandsPath"] : string.Empty;

            if (string.IsNullOrEmpty(unrealPakPath) || !File.Exists(unrealPakPath))
            {
                await MessageService.ShowWarning("UnrealPak executable path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(globalOutputPath))
            {
                await MessageService.ShowWarning("Please specify an output path.");
                return;
            }

            if (string.IsNullOrEmpty(cookedFilesPath) || !Directory.Exists(cookedFilesPath))
            {
                await MessageService.ShowWarning("Cooked files path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(packageStorePath) || !File.Exists(packageStorePath))
            {
                await MessageService.ShowWarning("PackageStore.manifest path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(scriptObjectsPath) || !File.Exists(scriptObjectsPath))
            {
                await MessageService.ShowWarning("ScriptObjects.bin path is missing or invalid.");
                return;
            }

            if (string.IsNullOrEmpty(ioStoreCommandsPath) || !File.Exists(ioStoreCommandsPath))
            {
                await MessageService.ShowWarning("IoStoreCommands.txt path is missing or invalid.");
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

        // Browse UnrealPak executable
        private void BrowseUnrealPakPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "UnrealPak.exe|UnrealPak.exe|All Files (*.*)|*.*",
                Title = "Select UnrealPak Executable"
            };

            var config = _configService.LoadUnrealPakConfig<dynamic>();
            string lastPath = config?.UnrealPakPath ?? string.Empty;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.UnrealPakPath = dialog.FileName;
                _configService.SaveUnrealPakConfig(config);
                UnrealPakPathTextBox.Text = dialog.FileName;
            }
        }

        // Browse global output path
        private void BrowseGlobalOutputPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select global.utoc Output Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            var config = _configService.LoadUnrealPakConfig<dynamic>();
            string lastPath = config?.GlobalOutputPath ?? string.Empty;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = lastPath;
            }

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    config.GlobalOutputPath = selectedPath;
                    _configService.SaveUnrealPakConfig(config);
                    GlobalOutputPathTextBox.Text = selectedPath;
                }
            }
        }

        // Browse cooked files path
        private void BrowseCookedFilesPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Folder Containing Cooked Files",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            var config = _configService.LoadUnrealPakConfig<dynamic>();
            string lastPath = config?.CookedFilesPath ?? string.Empty;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = lastPath;
            }

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    config.CookedFilesPath = selectedPath;
                    _configService.SaveUnrealPakConfig(config);
                    CookedFilesPathTextBox.Text = selectedPath;
                }
            }
        }

        // Browse packagestore.manifest path
        private void BrowsePackageStorePath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Package Store File (*.manifest, *json)|*.manifest;*.json|All Files (*.*)|*.*",
                Title = "Select Package Store File"
            };

            var config = _configService.LoadUnrealPakConfig<dynamic>();
            string lastPath = config?.PackageStorePath ?? string.Empty;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.PackageStorePath = dialog.FileName;
                _configService.SaveUnrealPakConfig(config);
                PackageStorePathTextBox.Text = dialog.FileName;
            }
        }

        // Browse ScriptObjects.bin path
        private void BrowseScriptObjectsPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "ScriptObjects.bin|ScriptObjects.bin|All Files (*.*)|*.*",
                Title = "Select ScriptObjects.bin File"
            };

            var config = _configService.LoadUnrealPakConfig<dynamic>();
            string lastPath = config?.ScriptObjectsPath ?? string.Empty;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.ScriptObjectsPath = dialog.FileName;
                _configService.SaveUnrealPakConfig(config);
                ScriptObjectsPathTextBox.Text = dialog.FileName;
            }
        }

        // Browse IoStoreCommands.txt path
        private void BrowseIoStoreCommandsPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Commands (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Select Commands.txt File"
            };

            var config = _configService.LoadUnrealPakConfig<dynamic>();
            string lastPath = config?.IoStoreCommandsPath ?? string.Empty;
            if (!string.IsNullOrEmpty(lastPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(lastPath);
            }

            if (dialog.ShowDialog() == true)
            {
                config.IoStoreCommandsPath = dialog.FileName;
                _configService.SaveUnrealPakConfig(config);
                IoStoreCommandsPathTextBox.Text = dialog.FileName;
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
                await MessageService.ShowError($"Error running command: {ex.Message}");
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
                await MessageService.ShowError($"Error running command: {ex.Message}");
            }
        }

        // Open Crypto.json in user's default app for .json files
        private async void OpenCryptoKeysFileAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                bool userConfirmed = await MessageService.ShowYesNo("Warning", "Are you sure you want to open Crypto.json?\n\nOnly edit this file if you know what you're doing.");

                if (userConfirmed)
                {
                    string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string filePath = Path.Combine(appDirectory, "configs", "Crypto.json");

                    if (File.Exists(filePath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        await MessageService.ShowError("Crypto.json file not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageService.ShowError($"Error opening file: {ex.Message}");
            }
        }

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

        // Repak Version Switch Dropdown
        private void ComboBox_RepakVersion(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (RepakVersionSwitchDropdown.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedVersion = selectedItem.Content.ToString();
                SaveGeneralConfigAsync(selectedVersion);
            }
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
                OpenIoStoreFlyout(sender, e);
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

        // Open IoStore Flyout
        private void OpenIoStoreFlyout(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.OpenIoStoreFlyout();
                LoadUnrealPakConfigAsync(); // Load config for unrealpak paths
            }
        }

        // Open AesKeys Flyout (Settings/Config)
        private void OpenAesKeysFlyout(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.OpenAesKeysFlyout();
                LoadAesKeysAsync(); // Load again here in case user changes the values via the config directly.
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
                IoStoreCommandOutputTextBox.Text += output + Environment.NewLine; // IoStore Packing output
            });
        }

        // Scroll CLI Outputs
        private void CliOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommandOutputTextBox.ScrollToEnd();
            IoStoreCommandOutputTextBox.ScrollToEnd();
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

        // Repak Settings Version Info
        public void LoadRepakVersionInfo()
        {
            List<RepakVersionInfo> repakVersionInfo = new List<RepakVersionInfo>
            {
                new RepakVersionInfo { UEVersion = "", Version = "1", VersionFeature = "Initial", Read = "?", Write = "?" },
                new RepakVersionInfo { UEVersion = "4.0-4.2", Version = "2", VersionFeature = "NoTimestamps", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "4.3-4.15", Version = "3", VersionFeature = "CompressionEncryption", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "4.16-4.19", Version = "4", VersionFeature = "IndexEncryption", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "4.20", Version = "5", VersionFeature = "RelativeChunkOffsets", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "", Version = "6", VersionFeature = "DeleteRecords", Read = "?", Write = "?" },
                new RepakVersionInfo { UEVersion = "4.21", Version = "7", VersionFeature = "EncryptionKeyGuid", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "4.22", Version = "8A", VersionFeature = "FNameBasedCompression", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "4.23-4.24", Version = "8B", VersionFeature = "FNameBasedCompression", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "4.25", Version = "9", VersionFeature = "FrozenIndex", Read = "✔", Write = "✔" },
                new RepakVersionInfo { UEVersion = "", Version = "10", VersionFeature = "PathHashIndex", Read = "?", Write = "?" },
                new RepakVersionInfo { UEVersion = "4.26-5.3", Version = "11", VersionFeature = "Fnv64BugFix", Read = "✔", Write = "✔" }
            };

            RepakDataGrid.ItemsSource = repakVersionInfo;
        }
    }
}
