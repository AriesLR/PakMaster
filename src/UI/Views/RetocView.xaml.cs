namespace PakMaster.UI.Views
{
    public partial class RetocView : UserControl
    {
        private bool isInitializing = false;
        private string inputFolderPath = string.Empty;
        private string outputFolderPath = string.Empty;

        public RetocView()
        {
            InitializeComponent();
            this.Loaded += RetocView_Loaded;
        }

        private void RetocView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigManager.CurrentSettings.RetocInfo != null)
            {
                LoadState();
            }
        }

        private void LoadState()
        {
            isInitializing = true;
            string cmd = ConfigManager.CurrentSettings.ActiveRetocCommand;

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

            LoadStateForCommand(cmd);

            isInitializing = false;
            UpdateCommand();
        }

        private void LoadStateForCommand(string cmd)
        {
            InputPathTextBox?.Text = string.Empty;
            OutputPathTextBox?.Text = string.Empty;
            TargetIDTextBox?.Text = string.Empty;
            AesKeyTextBox?.Text = string.Empty;
            HeaderVerComboBox?.SelectedIndex = 0;
            TocVerComboBox?.SelectedIndex = 0;
            EngineVerComboBox?.SelectedIndex = 0;
            FilterStrTextBox?.Text = string.Empty;
            ScriptCellTextBox?.Text = string.Empty;

            NoAssetsCheckBox?.IsChecked = false;
            NoShadersCheckBox?.IsChecked = false;
            NoScriptObjectsCheckBox?.IsChecked = false;
            NoCompressShadersCheckBox?.IsChecked = false;
            DryRunCheckBox?.IsChecked = false;
            VerboseCheckBox?.IsChecked = false;
            DebugCheckBox?.IsChecked = false;
            NoParallelCheckBox?.IsChecked = false;

            ListAllCheckBox?.IsChecked = false;
            ListHashCheckBox?.IsChecked = false;
            ListPackageCheckBox?.IsChecked = false;
            ListSizeCheckBox?.IsChecked = false;
            ListPathCheckBox?.IsChecked = false;
            ListStoreCheckBox?.IsChecked = false;

            var settings = ConfigManager.CurrentSettings;

            void SetHeader(string val)
            {
                if (HeaderVerComboBox != null)
                {
                    foreach (ComboBoxItem item in HeaderVerComboBox.Items)
                    {
                        if (item.Tag?.ToString() == val) { HeaderVerComboBox.SelectedItem = item; break; }
                    }
                }
            }
            void SetToc(string val)
            {
                if (TocVerComboBox != null)
                {
                    foreach (ComboBoxItem item in TocVerComboBox.Items)
                    {
                        if (item.Tag?.ToString() == val) { TocVerComboBox.SelectedItem = item; break; }
                    }
                }
            }
            void SetEngine(string val)
            {
                if (EngineVerComboBox != null)
                {
                    foreach (ComboBoxItem item in EngineVerComboBox.Items)
                    {
                        if (item.Content?.ToString() == val) { EngineVerComboBox.SelectedItem = item; break; }
                    }
                }
            }

            switch (cmd)
            {
                case "manifest":
                    InputPathTextBox?.Text = settings.RetocManifest.InputPath;
                    AesKeyTextBox?.Text = settings.RetocManifest.AesKey;
                    SetHeader(settings.RetocManifest.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocManifest.OverrideTocVersion);
                    break;

                case "info":
                    InputPathTextBox?.Text = settings.RetocInfo.InputPath;
                    AesKeyTextBox?.Text = settings.RetocInfo.AesKey;
                    SetHeader(settings.RetocInfo.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocInfo.OverrideTocVersion);
                    break;

                case "list":
                    InputPathTextBox?.Text = settings.RetocList.InputPath;
                    AesKeyTextBox?.Text = settings.RetocList.AesKey;
                    SetHeader(settings.RetocList.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocList.OverrideTocVersion);
                    ListAllCheckBox?.IsChecked = settings.RetocList.All;
                    ListHashCheckBox?.IsChecked = settings.RetocList.Hash;
                    ListPackageCheckBox?.IsChecked = settings.RetocList.Package;
                    ListSizeCheckBox?.IsChecked = settings.RetocList.Size;
                    ListPathCheckBox?.IsChecked = settings.RetocList.Path;
                    ListStoreCheckBox?.IsChecked = settings.RetocList.Store;
                    break;

                case "verify":
                    InputPathTextBox?.Text = settings.RetocVerify.InputPath;
                    AesKeyTextBox?.Text = settings.RetocVerify.AesKey;
                    SetHeader(settings.RetocVerify.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocVerify.OverrideTocVersion);
                    break;

                case "unpack":
                    InputPathTextBox?.Text = settings.RetocUnpack.InputPath;
                    OutputPathTextBox?.Text = settings.RetocUnpack.OutputPath;
                    AesKeyTextBox?.Text = settings.RetocUnpack.AesKey;
                    SetHeader(settings.RetocUnpack.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocUnpack.OverrideTocVersion);
                    VerboseCheckBox?.IsChecked = settings.RetocUnpack.Verbose;
                    break;

                case "unpack-raw":
                    InputPathTextBox?.Text = settings.RetocUnpackRaw.InputPath;
                    OutputPathTextBox?.Text = settings.RetocUnpackRaw.OutputPath;
                    AesKeyTextBox?.Text = settings.RetocUnpackRaw.AesKey;
                    SetHeader(settings.RetocUnpackRaw.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocUnpackRaw.OverrideTocVersion);
                    break;

                case "pack-raw":
                    InputPathTextBox?.Text = settings.RetocPackRaw.InputPath;
                    OutputPathTextBox?.Text = settings.RetocPackRaw.OutputPath;
                    AesKeyTextBox?.Text = settings.RetocPackRaw.AesKey;
                    SetHeader(settings.RetocPackRaw.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocPackRaw.OverrideTocVersion);
                    break;

                case "to-legacy":
                    InputPathTextBox?.Text = settings.RetocToLegacy.InputPath;
                    OutputPathTextBox?.Text = settings.RetocToLegacy.OutputPath;
                    AesKeyTextBox?.Text = settings.RetocToLegacy.AesKey;
                    SetHeader(settings.RetocToLegacy.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocToLegacy.OverrideTocVersion);
                    SetEngine(settings.RetocToLegacy.EngineVersion);
                    FilterStrTextBox?.Text = settings.RetocToLegacy.Filter;
                    ScriptCellTextBox?.Text = settings.RetocToLegacy.ScriptCell;
                    NoAssetsCheckBox?.IsChecked = settings.RetocToLegacy.NoAssets;
                    NoShadersCheckBox?.IsChecked = settings.RetocToLegacy.NoShaders;
                    NoScriptObjectsCheckBox?.IsChecked = settings.RetocToLegacy.NoScriptObjects;
                    NoCompressShadersCheckBox?.IsChecked = settings.RetocToLegacy.NoCompressShaders;
                    DryRunCheckBox?.IsChecked = settings.RetocToLegacy.DryRun;
                    VerboseCheckBox?.IsChecked = settings.RetocToLegacy.Verbose;
                    DebugCheckBox?.IsChecked = settings.RetocToLegacy.Debug;
                    NoParallelCheckBox?.IsChecked = settings.RetocToLegacy.NoParallel;
                    break;

                case "to-zen":
                    InputPathTextBox?.Text = settings.RetocToZen.InputPath;
                    OutputPathTextBox?.Text = settings.RetocToZen.OutputPath;
                    AesKeyTextBox?.Text = settings.RetocToZen.AesKey;
                    SetHeader(settings.RetocToZen.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocToZen.OverrideTocVersion);
                    SetEngine(settings.RetocToZen.EngineVersion);
                    FilterStrTextBox?.Text = settings.RetocToZen.Filter;
                    ScriptCellTextBox?.Text = settings.RetocToZen.ScriptCell;
                    VerboseCheckBox?.IsChecked = settings.RetocToZen.Verbose;
                    DebugCheckBox?.IsChecked = settings.RetocToZen.Debug;
                    NoParallelCheckBox?.IsChecked = settings.RetocToZen.NoParallel;
                    break;

                case "get":
                    InputPathTextBox?.Text = settings.RetocGet.InputPath;
                    OutputPathTextBox?.Text = settings.RetocGet.OutputPath;
                    TargetIDTextBox?.Text = settings.RetocGet.ChunkId;
                    AesKeyTextBox?.Text = settings.RetocGet.AesKey;
                    SetHeader(settings.RetocGet.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocGet.OverrideTocVersion);
                    break;

                case "dump-test":
                    InputPathTextBox?.Text = settings.RetocDumpTest.InputPath;
                    OutputPathTextBox?.Text = settings.RetocDumpTest.OutputPath;
                    TargetIDTextBox?.Text = settings.RetocDumpTest.PackageId;
                    AesKeyTextBox?.Text = settings.RetocDumpTest.AesKey;
                    SetHeader(settings.RetocDumpTest.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocDumpTest.OverrideTocVersion);
                    break;

                case "gen-script-objects":
                    InputPathTextBox?.Text = settings.RetocGenScriptObjects.InputPath;
                    OutputPathTextBox?.Text = settings.RetocGenScriptObjects.OutputPath;
                    AesKeyTextBox?.Text = settings.RetocGenScriptObjects.AesKey;
                    SetHeader(settings.RetocGenScriptObjects.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocGenScriptObjects.OverrideTocVersion);
                    SetEngine(settings.RetocGenScriptObjects.EngineVersion);
                    break;

                case "print-script-objects":
                    InputPathTextBox?.Text = settings.RetocPrintScriptObjects.InputPath;
                    AesKeyTextBox?.Text = settings.RetocPrintScriptObjects.AesKey;
                    SetHeader(settings.RetocPrintScriptObjects.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocPrintScriptObjects.OverrideTocVersion);
                    break;

                case "asset-registry":
                    InputPathTextBox?.Text = settings.RetocAssetRegistry.InputPath;
                    AesKeyTextBox?.Text = settings.RetocAssetRegistry.AesKey;
                    SetHeader(settings.RetocAssetRegistry.OverrideContainerHeaderVersion);
                    SetToc(settings.RetocAssetRegistry.OverrideTocVersion);
                    break;
            }

            UpdateWatermarks(cmd);
        }

        private void UpdateWatermarks(string cmd)
        {
            if (InputPathTextBox == null || OutputPathTextBox == null) return;

            string inputWatermark = "Input Path";
            string outputWatermark = "Output Path";

            InputFileBtnGrid?.Visibility = Visibility.Collapsed;
            InputFolderBtnGrid?.Visibility = Visibility.Collapsed;
            OutputFileBtnGrid?.Visibility = Visibility.Collapsed;
            OutputFolderBtnGrid?.Visibility = Visibility.Collapsed;

            switch (cmd)
            {
                case "manifest":
                case "list":
                case "verify":
                    inputWatermark = Lang.UTOC;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "info":
                    inputWatermark = Lang.PATH;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    InputFolderBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "print-script-objects":
                    inputWatermark = Lang.InputUtocFileContainingScriptObjects;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "asset-registry":
                    inputWatermark = Lang.InputAssetRegistryBinFile;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "unpack":
                case "unpack-raw":
                    inputWatermark = Lang.UTOC;
                    outputWatermark = Lang.OUTPUT;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    OutputFolderBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "to-legacy":
                    inputWatermark = Lang.InputUtocOrDirectoryWithMultipleUtocEGContentPaks;
                    outputWatermark = Lang.OutputDirectoryOrPak;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    InputFolderBtnGrid?.Visibility = Visibility.Visible;
                    OutputFileBtnGrid?.Visibility = Visibility.Visible;
                    OutputFolderBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "dump-test":
                    inputWatermark = Lang.INPUT;
                    outputWatermark = Lang.OUTPUT_DIR;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    InputFolderBtnGrid?.Visibility = Visibility.Visible;
                    OutputFolderBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "gen-script-objects":
                    inputWatermark = Lang.InputReflectionDataJmapDump;
                    outputWatermark = Lang.OutputUtocFile;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    OutputFileBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "pack-raw":
                    inputWatermark = Lang.INPUT;
                    outputWatermark = Lang.UTOC;
                    InputFolderBtnGrid?.Visibility = Visibility.Visible;
                    OutputFileBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "to-zen":
                    inputWatermark = Lang.InputDirectoryOrPak;
                    outputWatermark = Lang.OutputUtoc;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    InputFolderBtnGrid?.Visibility = Visibility.Visible;
                    OutputFileBtnGrid?.Visibility = Visibility.Visible;
                    break;

                case "get":
                    inputWatermark = Lang.InputUtocOrDirectoryWithMultipleUtocEGContentPaks;
                    outputWatermark = Lang.OptionalOutputPathOrStdoutIfOrOmitted;
                    InputFileBtnGrid?.Visibility = Visibility.Visible;
                    InputFolderBtnGrid?.Visibility = Visibility.Visible;
                    OutputFileBtnGrid?.Visibility = Visibility.Visible;
                    OutputFolderBtnGrid?.Visibility = Visibility.Visible;
                    break;
            }

            TextBoxHelper.SetWatermark(InputPathTextBox, inputWatermark);
            TextBoxHelper.SetWatermark(OutputPathTextBox, outputWatermark);
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
                string cmd = (CmdSelectComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "info";
                ConfigManager.CurrentSettings.ActiveRetocCommand = cmd;

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

            string cmd = (CmdSelectComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "info";
            string inputPath = InputPathTextBox?.Text ?? string.Empty;
            string outputPath = OutputPathTextBox?.Text ?? string.Empty;
            string aesKey = AesKeyTextBox?.Text ?? string.Empty;
            string targetId = TargetIDTextBox?.Text ?? string.Empty;

            string headerVer = (HeaderVerComboBox?.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;
            string tocVer = (TocVerComboBox?.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;
            string engineVer = (EngineVerComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

            string filterStr = FilterStrTextBox?.Text ?? string.Empty;
            string scriptCell = ScriptCellTextBox?.Text ?? string.Empty;

            bool noAssets = NoAssetsCheckBox?.IsChecked ?? false;
            bool noShaders = NoShadersCheckBox?.IsChecked ?? false;
            bool noScriptObjects = NoScriptObjectsCheckBox?.IsChecked ?? false;
            bool noCompressShaders = NoCompressShadersCheckBox?.IsChecked ?? false;
            bool dryRun = DryRunCheckBox?.IsChecked ?? false;

            bool verbose = VerboseCheckBox?.IsChecked ?? false;
            bool debug = DebugCheckBox?.IsChecked ?? false;
            bool noParallel = NoParallelCheckBox?.IsChecked ?? false;

            bool listAll = ListAllCheckBox?.IsChecked ?? false;
            bool listHash = ListHashCheckBox?.IsChecked ?? false;
            bool listPackage = ListPackageCheckBox?.IsChecked ?? false;
            bool listSize = ListSizeCheckBox?.IsChecked ?? false;
            bool listPath = ListPathCheckBox?.IsChecked ?? false;
            bool listStore = ListStoreCheckBox?.IsChecked ?? false;

            var settings = ConfigManager.CurrentSettings;
            settings.ActiveRetocCommand = cmd;

            switch (cmd)
            {
                case "manifest":
                    settings.RetocManifest.InputPath = inputPath;
                    settings.RetocManifest.AesKey = aesKey;
                    settings.RetocManifest.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocManifest.OverrideTocVersion = tocVer;
                    break;

                case "info":
                    settings.RetocInfo.InputPath = inputPath;
                    settings.RetocInfo.AesKey = aesKey;
                    settings.RetocInfo.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocInfo.OverrideTocVersion = tocVer;
                    break;

                case "list":
                    settings.RetocList.InputPath = inputPath;
                    settings.RetocList.AesKey = aesKey;
                    settings.RetocList.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocList.OverrideTocVersion = tocVer;
                    settings.RetocList.All = listAll;
                    settings.RetocList.Hash = listHash;
                    settings.RetocList.Package = listPackage;
                    settings.RetocList.Size = listSize;
                    settings.RetocList.Path = listPath;
                    settings.RetocList.Store = listStore;
                    break;

                case "verify":
                    settings.RetocVerify.InputPath = inputPath;
                    settings.RetocVerify.AesKey = aesKey;
                    settings.RetocVerify.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocVerify.OverrideTocVersion = tocVer;
                    break;

                case "unpack":
                    settings.RetocUnpack.InputPath = inputPath;
                    settings.RetocUnpack.OutputPath = outputPath;
                    settings.RetocUnpack.AesKey = aesKey;
                    settings.RetocUnpack.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocUnpack.OverrideTocVersion = tocVer;
                    settings.RetocUnpack.Verbose = verbose;
                    break;

                case "unpack-raw":
                    settings.RetocUnpackRaw.InputPath = inputPath;
                    settings.RetocUnpackRaw.OutputPath = outputPath;
                    settings.RetocUnpackRaw.AesKey = aesKey;
                    settings.RetocUnpackRaw.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocUnpackRaw.OverrideTocVersion = tocVer;
                    break;

                case "pack-raw":
                    settings.RetocPackRaw.InputPath = inputPath;
                    settings.RetocPackRaw.OutputPath = outputPath;
                    settings.RetocPackRaw.AesKey = aesKey;
                    settings.RetocPackRaw.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocPackRaw.OverrideTocVersion = tocVer;
                    break;

                case "to-legacy":
                    settings.RetocToLegacy.InputPath = inputPath;
                    settings.RetocToLegacy.OutputPath = outputPath;
                    settings.RetocToLegacy.AesKey = aesKey;
                    settings.RetocToLegacy.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocToLegacy.OverrideTocVersion = tocVer;
                    settings.RetocToLegacy.EngineVersion = engineVer;
                    settings.RetocToLegacy.Filter = filterStr;
                    settings.RetocToLegacy.ScriptCell = scriptCell;
                    settings.RetocToLegacy.NoAssets = noAssets;
                    settings.RetocToLegacy.NoShaders = noShaders;
                    settings.RetocToLegacy.NoScriptObjects = noScriptObjects;
                    settings.RetocToLegacy.NoCompressShaders = noCompressShaders;
                    settings.RetocToLegacy.DryRun = dryRun;
                    settings.RetocToLegacy.Verbose = verbose;
                    settings.RetocToLegacy.Debug = debug;
                    settings.RetocToLegacy.NoParallel = noParallel;
                    break;

                case "to-zen":
                    settings.RetocToZen.InputPath = inputPath;
                    settings.RetocToZen.OutputPath = outputPath;
                    settings.RetocToZen.AesKey = aesKey;
                    settings.RetocToZen.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocToZen.OverrideTocVersion = tocVer;
                    settings.RetocToZen.EngineVersion = engineVer;
                    settings.RetocToZen.Filter = filterStr;
                    settings.RetocToZen.ScriptCell = scriptCell;
                    settings.RetocToZen.Verbose = verbose;
                    settings.RetocToZen.Debug = debug;
                    settings.RetocToZen.NoParallel = noParallel;
                    break;

                case "get":
                    settings.RetocGet.InputPath = inputPath;
                    settings.RetocGet.OutputPath = outputPath;
                    settings.RetocGet.ChunkId = targetId;
                    settings.RetocGet.AesKey = aesKey;
                    settings.RetocGet.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocGet.OverrideTocVersion = tocVer;
                    break;

                case "dump-test":
                    settings.RetocDumpTest.InputPath = inputPath;
                    settings.RetocDumpTest.OutputPath = outputPath;
                    settings.RetocDumpTest.PackageId = targetId;
                    settings.RetocDumpTest.AesKey = aesKey;
                    settings.RetocDumpTest.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocDumpTest.OverrideTocVersion = tocVer;
                    break;

                case "gen-script-objects":
                    settings.RetocGenScriptObjects.InputPath = inputPath;
                    settings.RetocGenScriptObjects.OutputPath = outputPath;
                    settings.RetocGenScriptObjects.AesKey = aesKey;
                    settings.RetocGenScriptObjects.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocGenScriptObjects.OverrideTocVersion = tocVer;
                    settings.RetocGenScriptObjects.EngineVersion = engineVer;
                    break;

                case "print-script-objects":
                    settings.RetocPrintScriptObjects.InputPath = inputPath;
                    settings.RetocPrintScriptObjects.AesKey = aesKey;
                    settings.RetocPrintScriptObjects.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocPrintScriptObjects.OverrideTocVersion = tocVer;
                    break;

                case "asset-registry":
                    settings.RetocAssetRegistry.InputPath = inputPath;
                    settings.RetocAssetRegistry.AesKey = aesKey;
                    settings.RetocAssetRegistry.OverrideContainerHeaderVersion = headerVer;
                    settings.RetocAssetRegistry.OverrideTocVersion = tocVer;
                    break;
            }

            ConfigManager.SaveConfig(settings);

            bool needsOutput = new[] { "unpack", "unpack-raw", "pack-raw", "to-legacy", "to-zen", "get", "dump-test", "gen-script-objects" }.Contains(cmd);
            OutputPathGroup?.Visibility = needsOutput ? Visibility.Visible : Visibility.Collapsed;

            bool needsTargetId = new[] { "get", "dump-test" }.Contains(cmd);
            TargetIDGroup?.Visibility = needsTargetId ? Visibility.Visible : Visibility.Collapsed;
            TargetIDLabel?.Text = cmd == "dump-test" ? "Package ID" : "Chunk ID";

            bool isList = cmd == "list";
            ListOptionsGroup?.Visibility = isList ? Visibility.Visible : Visibility.Collapsed;

            bool isLegacy = cmd == "to-legacy";
            bool isZen = cmd == "to-zen";
            ConversionOptionsGroup?.Visibility = (isLegacy || isZen) ? Visibility.Visible : Visibility.Collapsed;

            Visibility legacyCheckboxVis = isLegacy ? Visibility.Visible : Visibility.Collapsed;
            DryRunCheckBox?.Visibility = legacyCheckboxVis;
            NoAssetsCheckBox?.Visibility = legacyCheckboxVis;
            NoShadersCheckBox?.Visibility = legacyCheckboxVis;
            NoScriptObjectsCheckBox?.Visibility = legacyCheckboxVis;
            NoCompressShadersCheckBox?.Visibility = legacyCheckboxVis;

            bool needsEngineVer = new[] { "to-legacy", "to-zen", "gen-script-objects" }.Contains(cmd);
            EngineVerGroup?.Visibility = needsEngineVer ? Visibility.Visible : Visibility.Collapsed;

            bool needsFilter = new[] { "to-legacy", "to-zen" }.Contains(cmd);
            FilterGroup?.Visibility = needsFilter ? Visibility.Visible : Visibility.Collapsed;

            bool needsVerbose = new[] { "unpack", "to-legacy", "to-zen" }.Contains(cmd);
            VerboseCheckBox?.Visibility = needsVerbose ? Visibility.Visible : Visibility.Collapsed;

            bool needsDebugParallel = new[] { "to-legacy", "to-zen" }.Contains(cmd);
            DebugCheckBox?.Visibility = needsDebugParallel ? Visibility.Visible : Visibility.Collapsed;
            NoParallelCheckBox?.Visibility = needsDebugParallel ? Visibility.Visible : Visibility.Collapsed;

            GlobalOptionsGroup?.Visibility = Visibility.Visible;

            CmdPreviewTextBox.Text = PakMaster.Core.Engines.RetocEngine.BuildCommandString(settings);
        }

        private void BrowseInputFile_Click(object sender, RoutedEventArgs e)
        {
            string cmd = (CmdSelectComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "info";

            bool wantsJmap = cmd == "gen-script-objects";
            bool wantsBin = cmd == "asset-registry";
            bool wantsPak = cmd == "to-zen";
            bool wantsUtoc = new[] { "manifest", "list", "verify", "unpack", "unpack-raw", "to-legacy", "get", "print-script-objects" }.Contains(cmd);

            string filter = "Unreal Files (*.utoc;*.pak)|*.utoc;*.pak|All Files (*.*)|*.*";
            if (wantsJmap) filter = "Jmap Dump (*.jmap)|*.jmap|All Files (*.*)|*.*";
            else if (wantsBin) filter = "Asset Registry (AssetRegistry.bin)|AssetRegistry.bin|All Files (*.*)|*.*";
            else if (wantsPak) filter = "PAK Files (*.pak)|*.pak|All Files (*.*)|*.*";
            else if (wantsUtoc) filter = "UTOC Files (*.utoc)|*.utoc|All Files (*.*)|*.*";

            var openFileDialog = new OpenFileDialog
            {
                Title = Lang.SelectInputFile,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = filter
            };

            if (!string.IsNullOrEmpty(inputFolderPath)) openFileDialog.InitialDirectory = inputFolderPath;

            if (openFileDialog.ShowDialog() == true)
            {
                inputFolderPath = Path.GetDirectoryName(openFileDialog.FileName) ?? string.Empty;
                InputPathTextBox?.Text = openFileDialog.FileName;
            }
        }

        private void BrowseInputFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog { Title = Lang.SelectInputFolder };
            if (!string.IsNullOrEmpty(inputFolderPath)) folderDialog.InitialDirectory = inputFolderPath;

            if (folderDialog.ShowDialog() == true)
            {
                inputFolderPath = folderDialog.FolderName;
                InputPathTextBox?.Text = inputFolderPath;
            }
        }

        private void BrowseOutputFile_Click(object sender, RoutedEventArgs e)
        {
            string cmd = (CmdSelectComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "info";

            bool wantsUtocFile = cmd == "pack-raw" || cmd == "to-zen" || cmd == "gen-script-objects";
            bool wantsPakFile = cmd == "to-legacy";

            var saveDialog = new SaveFileDialog
            {
                Title = Lang.SelectOutputFile,
                Filter = wantsUtocFile ? "UTOC Files (*.utoc)|*.utoc" : wantsPakFile ? "PAK Files (*.pak)|*.pak" : "All Files (*.*)|*.*",
                DefaultExt = wantsUtocFile ? ".utoc" : wantsPakFile ? ".pak" : "",
                ValidateNames = false,
                CheckPathExists = false
            };

            if (!string.IsNullOrEmpty(outputFolderPath)) saveDialog.InitialDirectory = outputFolderPath;

            if (saveDialog.ShowDialog() == true)
            {
                outputFolderPath = Path.GetDirectoryName(saveDialog.FileName) ?? string.Empty;
                OutputPathTextBox?.Text = saveDialog.FileName;
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog { Title = Lang.SelectOutputFolder };
            if (!string.IsNullOrEmpty(outputFolderPath)) folderDialog.InitialDirectory = outputFolderPath;

            if (folderDialog.ShowDialog() == true)
            {
                outputFolderPath = folderDialog.FolderName;
                OutputPathTextBox?.Text = outputFolderPath;
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterStrTextBox?.Text = string.Empty;
        }

        private void ClearAesKey_Click(object sender, RoutedEventArgs e)
        {
            AesKeyTextBox?.Text = string.Empty;
        }

        private void ClearScriptCell_Click(object sender, RoutedEventArgs e)
        {
            ScriptCellTextBox?.Text = string.Empty;
        }

        private void ClearTargetID_Click(object sender, RoutedEventArgs e)
        {
            TargetIDTextBox?.Text = string.Empty;
        }

        private async void ExecuteCommand_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CmdPreviewTextBox?.Text)) return;

            await RetocEngine.ExecuteCommandAsync(CmdPreviewTextBox.Text, output =>
            { });
        }
    }
}