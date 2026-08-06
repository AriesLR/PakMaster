namespace PakMaster.UI.Views
{
    public partial class RepakView : UserControl
    {
        private bool isInitializing = false;
        private string inputFolderPath = string.Empty;
        private string outputFolderPath = string.Empty;

        public RepakView()
        {
            InitializeComponent();
            this.Loaded += RepakView_Loaded;
            this.Unloaded += RepakView_Unloaded;
            ToolDependencyEngine.PackagesUpdated += ToolDependencyEngine_PackagesUpdated;
            ConfigManager.ProfileChanged += ConfigManager_ProfileChanged;
        }

        private void RepakView_Unloaded(object sender, RoutedEventArgs e)
        {
            ToolDependencyEngine.PackagesUpdated -= ToolDependencyEngine_PackagesUpdated;
            ConfigManager.ProfileChanged -= ConfigManager_ProfileChanged;
        }

        private void ConfigManager_ProfileChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (IsLoaded)
                {
                    LoadState();
                }
            });
        }

        private void ToolDependencyEngine_PackagesUpdated()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (IsLoaded)
                {
                    LoadState();
                }
            });
        }

        private void RepakView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigManager.CurrentSettings.RepakPack != null)
            {
                LoadState();
            }
        }

        private void LoadState()
        {
            isInitializing = true;
            string cmd = ConfigManager.CurrentSettings.ActiveRepakCommand;
            string branch = ConfigManager.CurrentSettings.ActiveRepakBranch;

            if (CmdSelectComboBox != null)
            {
                foreach (ComboBoxItem item in CmdSelectComboBox.Items)
                {
                    if (item.Content?.ToString() == cmd)
                    {
                        CmdSelectComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (BranchSelectComboBox != null)
            {
                BranchSelectComboBox.Items.Clear();
                var branches = ToolDependencyEngine.GetAvailableBranches("Repak");
                foreach (var b in branches)
                {
                    BranchSelectComboBox.Items.Add(new ComboBoxItem { Content = b.DisplayName });
                }

                foreach (ComboBoxItem item in BranchSelectComboBox.Items)
                {
                    if (item.Content?.ToString() == branch)
                    {
                        BranchSelectComboBox.SelectedItem = item;
                        break;
                    }
                }
                
                if (BranchSelectComboBox.SelectedItem == null && BranchSelectComboBox.Items.Count > 0)
                {
                    BranchSelectComboBox.SelectedIndex = 0;
                }
            }

            LoadStateForCommand(cmd);

            isInitializing = false;
            UpdateCommand();
        }

        private void BranchSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || isInitializing) return;

            if (BranchSelectComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                ConfigManager.CurrentSettings.ActiveRepakBranch = selectedItem.Content.ToString() ?? "main";
                ConfigManager.SaveConfig(ConfigManager.CurrentSettings);
            }
        }

        private void LoadStateForCommand(string cmd)
        {
            InputPathTextBox?.Text = string.Empty;
            OutputPathTextBox?.Text = string.Empty;
            AesKeyTextBox?.Text = string.Empty;
            MountPointTextBox?.Text = string.Empty;
            PathHashSeedTextBox?.Text = string.Empty;
            ForceCheckBox?.IsChecked = false;
            IncludeTextBox?.Text = string.Empty;
            StripPrefixTextBox?.Text = string.Empty;
            VerboseCheckBox?.IsChecked = false;
            QuietCheckBox?.IsChecked = false;
            GetFileTextBox?.Text = string.Empty;
            CompressionComboBox?.SelectedIndex = 0;
            RepakVersionSwitchDropdown?.SelectedIndex = 0;

            var settings = ConfigManager.CurrentSettings;
            switch (cmd)
            {
                case "info":
                    InputPathTextBox?.Text = settings.RepakInfo.InputPath;
                    AesKeyTextBox?.Text = settings.RepakInfo.AesKey;
                    break;

                case "list":
                    InputPathTextBox?.Text = settings.RepakList.InputPath;
                    StripPrefixTextBox?.Text = settings.RepakList.StripPrefix;
                    AesKeyTextBox?.Text = settings.RepakList.AesKey;
                    break;

                case "hash-list":
                    InputPathTextBox?.Text = settings.RepakHashList.InputPath;
                    StripPrefixTextBox?.Text = settings.RepakHashList.StripPrefix;
                    AesKeyTextBox?.Text = settings.RepakHashList.AesKey;
                    break;

                case "unpack":
                    InputPathTextBox?.Text = settings.RepakUnpack.InputPath;
                    OutputPathTextBox?.Text = settings.RepakUnpack.OutputPath;
                    StripPrefixTextBox?.Text = settings.RepakUnpack.StripPrefix;
                    AesKeyTextBox?.Text = settings.RepakUnpack.AesKey;
                    IncludeTextBox?.Text = settings.RepakUnpack.Include;
                    ForceCheckBox?.IsChecked = settings.RepakUnpack.Force;
                    VerboseCheckBox?.IsChecked = settings.RepakUnpack.Verbose;
                    QuietCheckBox?.IsChecked = settings.RepakUnpack.Quiet;
                    break;

                case "pack":
                    InputPathTextBox?.Text = settings.RepakPack.InputPath;
                    OutputPathTextBox?.Text = settings.RepakPack.OutputPath;
                    MountPointTextBox?.Text = settings.RepakPack.MountPoint;
                    PathHashSeedTextBox?.Text = settings.RepakPack.PathHashSeed;
                    VerboseCheckBox?.IsChecked = settings.RepakPack.Verbose;
                    QuietCheckBox?.IsChecked = settings.RepakPack.Quiet;
                    AesKeyTextBox?.Text = settings.RepakPack.AesKey;
                    if (CompressionComboBox != null)
                    {
                        foreach (ComboBoxItem item in CompressionComboBox.Items)
                            if (item.Content?.ToString() == settings.RepakPack.Compression) { CompressionComboBox.SelectedItem = item; break; }
                    }
                    if (RepakVersionSwitchDropdown != null)
                    {
                        foreach (ComboBoxItem item in RepakVersionSwitchDropdown.Items)
                            if (item.Content?.ToString() == settings.RepakPack.RepakVersion) { RepakVersionSwitchDropdown.SelectedItem = item; break; }
                    }
                    break;

                case "get":
                    InputPathTextBox?.Text = settings.RepakGet.InputPath;
                    GetFileTextBox?.Text = settings.RepakGet.GetFile;
                    StripPrefixTextBox?.Text = settings.RepakGet.StripPrefix;
                    AesKeyTextBox?.Text = settings.RepakGet.AesKey;
                    break;
            }
        }

        private void UpdateCommand_Event(object sender, RoutedEventArgs e)
        { if (IsLoaded) UpdateCommand(); }

        private void UpdateCommand_TextChanged(object sender, TextChangedEventArgs e)
        { if (IsLoaded) UpdateCommand(); }

        private void UpdateCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (sender == CmdSelectComboBox)
            {
                string cmd = (CmdSelectComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "unpack";
                ConfigManager.CurrentSettings.ActiveRepakCommand = cmd;

                bool oldInit = isInitializing;
                isInitializing = true;
                LoadStateForCommand(cmd);
                isInitializing = oldInit;
            }
            if (!isInitializing) UpdateCommand();
        }

        private void UpdateCommand()
        {
            if (isInitializing) return;
            if (CmdSelectComboBox == null || CmdPreviewTextBox == null || !IsLoaded) return;

            string cmd = (CmdSelectComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "unpack";
            string inputPath = InputPathTextBox?.Text ?? string.Empty;
            string outputPath = OutputPathTextBox?.Text ?? string.Empty;
            string aesKey = AesKeyTextBox?.Text ?? string.Empty;
            string repakVersion = (RepakVersionSwitchDropdown?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "V11";
            string mountPoint = MountPointTextBox?.Text ?? string.Empty;
            string compression = (CompressionComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            string pathHashSeed = PathHashSeedTextBox?.Text ?? string.Empty;
            bool force = ForceCheckBox?.IsChecked ?? false;
            string includeStr = IncludeTextBox?.Text ?? string.Empty;
            string stripPrefix = StripPrefixTextBox?.Text ?? string.Empty;
            bool verbose = VerboseCheckBox?.IsChecked ?? false;
            bool quiet = QuietCheckBox?.IsChecked ?? false;
            string getFile = GetFileTextBox?.Text ?? string.Empty;

            var settings = ConfigManager.CurrentSettings;
            settings.ActiveRepakCommand = cmd;

            switch (cmd)
            {
                case "info":
                    settings.RepakInfo.InputPath = inputPath;
                    settings.RepakInfo.AesKey = aesKey;
                    break;

                case "list":
                    settings.RepakList.InputPath = inputPath;
                    settings.RepakList.StripPrefix = stripPrefix;
                    settings.RepakList.AesKey = aesKey;
                    break;

                case "hash-list":
                    settings.RepakHashList.InputPath = inputPath;
                    settings.RepakHashList.StripPrefix = stripPrefix;
                    settings.RepakHashList.AesKey = aesKey;
                    break;

                case "unpack":
                    settings.RepakUnpack.InputPath = inputPath;
                    settings.RepakUnpack.OutputPath = outputPath;
                    settings.RepakUnpack.StripPrefix = stripPrefix;
                    settings.RepakUnpack.AesKey = aesKey;
                    settings.RepakUnpack.Include = includeStr;
                    settings.RepakUnpack.Force = force;
                    settings.RepakUnpack.Verbose = verbose;
                    settings.RepakUnpack.Quiet = quiet;
                    break;

                case "pack":
                    settings.RepakPack.InputPath = inputPath;
                    settings.RepakPack.OutputPath = outputPath;
                    settings.RepakPack.MountPoint = mountPoint;
                    settings.RepakPack.PathHashSeed = pathHashSeed;
                    settings.RepakPack.RepakVersion = repakVersion;
                    settings.RepakPack.Compression = compression;
                    settings.RepakPack.Verbose = verbose;
                    settings.RepakPack.Quiet = quiet;
                    settings.RepakPack.AesKey = aesKey;
                    break;

                case "get":
                    settings.RepakGet.InputPath = inputPath;
                    settings.RepakGet.GetFile = getFile;
                    settings.RepakGet.StripPrefix = stripPrefix;
                    settings.RepakGet.AesKey = aesKey;
                    break;
            }

            ConfigManager.SaveConfig(settings);

            bool needsOutput = new[] { "unpack", "pack" }.Contains(cmd);
            bool isPack = cmd == "pack";
            bool isUnpack = cmd == "unpack";
            bool isGet = cmd == "get";

            OutputPathGroup?.Visibility = needsOutput ? Visibility.Visible : Visibility.Collapsed;
            GetFileGroup?.Visibility = isGet ? Visibility.Visible : Visibility.Collapsed;
            UnpackOptionsGroup?.Visibility = isUnpack ? Visibility.Visible : Visibility.Collapsed;
            InputBrowseBtnText?.Text = isPack ? Lang.RetocView_Folder : Lang.RetocView_File;
            OutputBrowseBtnText?.Text = isPack ? Lang.RetocView_File : Lang.RetocView_Folder;
            PackOptionsGroup?.Visibility = isPack ? Visibility.Visible : Visibility.Collapsed;
            StripPrefixGroup?.Visibility = new[] { "unpack", "list", "hash-list", "get" }.Contains(cmd) ? Visibility.Visible : Visibility.Collapsed;

            Visibility showForPackUnpack = (isPack || isUnpack) ? Visibility.Visible : Visibility.Collapsed;
            RepakVersionGroup?.Visibility = isPack ? Visibility.Visible : Visibility.Collapsed;
            VerboseCheckBox?.Visibility = showForPackUnpack;
            QuietCheckBox?.Visibility = showForPackUnpack;

            AesKeyGroup?.Visibility = Visibility.Visible;
            GlobalOptionsGroup?.Visibility = Visibility.Visible;

            CmdPreviewTextBox.Text = RepakEngine.BuildCommandString(settings);
        }

        private void BrowseInputFolder_Click(object sender, RoutedEventArgs e)
        {
            string cmd = (CmdSelectComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "unpack";
            bool isPack = cmd == "pack";

            if (isPack)
            {
                var folderDialog = new OpenFolderDialog
                {
                    Title = Lang.SelectAFolderToPack
                };
                if (!string.IsNullOrEmpty(inputFolderPath)) folderDialog.InitialDirectory = inputFolderPath;

                if (folderDialog.ShowDialog() == true)
                {
                    inputFolderPath = folderDialog.FolderName;
                    InputPathTextBox?.Text = inputFolderPath;
                }
            }
            else
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = Lang.SelectAPakFile,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Filter = "Pak Files (*.pak)|*.pak|All Files (*.*)|*.*"
                };
                if (!string.IsNullOrEmpty(inputFolderPath)) openFileDialog.InitialDirectory = inputFolderPath;

                if (openFileDialog.ShowDialog() == true)
                {
                    inputFolderPath = Path.GetDirectoryName(openFileDialog.FileName) ?? string.Empty;
                    InputPathTextBox?.Text = openFileDialog.FileName;
                }
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            string cmd = (CmdSelectComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "unpack";
            bool isPack = cmd == "pack";

            if (isPack)
            {
                var saveDialog = new SaveFileDialog
                {
                    Title = Lang.SelectOutputPakFile,
                    Filter = "Pak Files (*.pak)|*.pak",
                    DefaultExt = ".pak",
                    ValidateNames = false,
                    CheckPathExists = false
                };
                if (!string.IsNullOrEmpty(outputFolderPath))
                {
                    saveDialog.InitialDirectory = outputFolderPath;
                }
                if (!string.IsNullOrEmpty(inputFolderPath))
                {
                    saveDialog.FileName = Path.GetFileName(inputFolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) + ".pak";
                }
                if (saveDialog.ShowDialog() == true)
                {
                    outputFolderPath = Path.GetDirectoryName(saveDialog.FileName) ?? string.Empty;
                    OutputPathTextBox?.Text = saveDialog.FileName;
                }
            }
            else
            {
                var folderDialog = new OpenFolderDialog
                {
                    Title = Lang.SelectAFolderForOutput
                };
                if (!string.IsNullOrEmpty(outputFolderPath))
                {
                    folderDialog.InitialDirectory = outputFolderPath;
                }
                if (folderDialog.ShowDialog() == true)
                {
                    outputFolderPath = folderDialog.FolderName;
                    OutputPathTextBox?.Text = outputFolderPath;
                }
            }
        }

        private void ClearGetFile_Click(object sender, RoutedEventArgs e)
        {
            GetFileTextBox?.Text = string.Empty;
        }

        private void ClearAesKey_Click(object sender, RoutedEventArgs e)
        {
            AesKeyTextBox?.Text = string.Empty;
        }

        private void ClearStripPrefix_Click(object sender, RoutedEventArgs e)
        {
            StripPrefixTextBox?.Text = string.Empty;
        }

        private void ClearInclude_Click(object sender, RoutedEventArgs e)
        {
            IncludeTextBox?.Text = string.Empty;
        }

        private void ClearMountPoint_Click(object sender, RoutedEventArgs e)
        {
            MountPointTextBox?.Text = string.Empty;
        }

        private void ClearPathHashSeed_Click(object sender, RoutedEventArgs e)
        {
            MountPointTextBox?.Text = string.Empty;
        }

        private void ComboBox_RepakVersion(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                UpdateCommand();
            }
        }

        private async void ExecuteCommand_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CmdPreviewTextBox?.Text)) return;

            await RepakEngine.ExecuteCommandAsync(CmdPreviewTextBox.Text, output =>
            { });
        }
    }
}