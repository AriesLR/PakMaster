namespace PakMaster.UI.Views
{
    public partial class MainView : UserControl
    {
        private bool isIoStoreMode = false;
        private string? inputFolderPath;
        private string? outputFolderPath;

        public MainView()
        {
            InitializeComponent();
            DataContext = new MainWindowState();
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

        // Refresh Button
        private void RefreshUI_Click(object sender, RoutedEventArgs e)
        {
            RepopulateInputListBox();
            RepopulateOutputListBox();
        }

        // Mode Switch Button - Normal
        private void ModeSwitchNormal_Click(object sender, RoutedEventArgs e)
        {
            ModeSwitchButton.Content = Lang.MainView_ModeSwitch_NormalMode;
            isIoStoreMode = false; // Normal Mode (.pak)
            RefreshUI();
        }

        // Mode Switch Button - IoStore
        private void ModeSwitchIoStore_Click(object sender, RoutedEventArgs e)
        {
            ModeSwitchButton.Content = Lang.MainView_ModeSwitch_IoStoreMode;
            isIoStoreMode = true; // IoStore Mode (.ucas/.utoc)
            RefreshUI();
        }

        // Unpack Button
        private async void Unpack_Click(object sender, RoutedEventArgs e)
        {
            if (isIoStoreMode)
            {
                await ZenToolsEngine.UnpackAsync(inputFolderPath ?? string.Empty, outputFolderPath ?? string.Empty, output =>
                {
                    UpdateCommandOutput(output);
                    RefreshUI();
                });
            }
            else
            {
                var selectedInputFile = InputFilesListBox.SelectedItem as KeyValuePair<string, string>?;
                string fullInputFilePath = selectedInputFile.HasValue ? selectedInputFile.Value.Value : string.Empty;

                await RepakEngine.UnpackAsync(fullInputFilePath, outputFolderPath ?? string.Empty, output =>
                {
                    UpdateCommandOutput(output);
                    RefreshUI();
                });
            }
        }

        // Repack Button
        private async void Repack_Click(object sender, RoutedEventArgs e)
        {
            if (isIoStoreMode)
            {
                if (Application.Current.MainWindow.DataContext is MainWindowState state)
                {
                    state.OpenIoStoreFlyout();
                }
            }
            else
            {
                var selectedInputFolder = OutputFilesListBox.SelectedItem as KeyValuePair<string, string>?;
                string fullInputFolderPath = selectedInputFolder.HasValue ? selectedInputFolder.Value.Value : string.Empty;

                await RepakEngine.RepackAsync(fullInputFolderPath, inputFolderPath ?? string.Empty, output =>
                {
                    UpdateCommandOutput(output);
                    RefreshUI();
                });
            }
        }

        // Open AesKeys Flyout (Settings/Config)
        private void OpenAesKeysFlyout(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowState viewModel)
            {
                viewModel.OpenAesKeysFlyout();
            }
        }

        // Refresh UI Elements
        public void RefreshUI()
        {
            RepopulateInputListBox();
            RepopulateOutputListBox();
        }

        // Update CLI Output
        public void UpdateCommandOutput(string output)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommandOutputTextBox.Text += output + Environment.NewLine;
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