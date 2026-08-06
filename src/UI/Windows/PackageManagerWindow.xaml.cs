namespace PakMaster.UI.Windows
{
    public partial class PackageManagerWindow : MetroWindow
    {
        public PackageManagerWindow()
        {
            InitializeComponent();
            RefreshLists();
        }

        private bool _isSafeToClose = false;

        private void RefreshLists()
        {
            ToolDependencyEngine.UpdatePackageStates();
            var allPackages = ToolDependencyEngine.GetAllPackages();

            foreach (var package in allPackages)
            {
                package.PendingInstallState = package.IsInstalled;
            }

            RepakList.ItemsSource = null;
            RepakList.ItemsSource = allPackages.Where(p => p.ToolType.Equals("Repak", StringComparison.OrdinalIgnoreCase)).ToList();

            RetocList.ItemsSource = null;
            RetocList.ItemsSource = allPackages.Where(p => p.ToolType.Equals("Retoc", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void SelectAllRetoc_Checked(object sender, RoutedEventArgs e)
        {
            SetAllPendingInstallState("Retoc", true);
        }

        private void SelectAllRetoc_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllPendingInstallState("Retoc", false);
        }

        private void SelectAllRepak_Checked(object sender, RoutedEventArgs e)
        {
            SetAllPendingInstallState("Repak", true);
        }

        private void SelectAllRepak_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllPendingInstallState("Repak", false);
        }

        private void SetAllPendingInstallState(string toolType, bool state)
        {
            if (toolType.Equals("Retoc", StringComparison.OrdinalIgnoreCase))
            {
                if (RetocList.ItemsSource is List<PackageModel> items)
                {
                    foreach (var pkg in items)
                    {
                        pkg.PendingInstallState = state;
                    }
                }
                RetocList.Items.Refresh();
            }
            else
            {
                if (RepakList.ItemsSource is List<PackageModel> items)
                {
                    foreach (var pkg in items)
                    {
                        pkg.PendingInstallState = state;
                    }
                }
                RepakList.Items.Refresh();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isSafeToClose) return;

            var allPackages = ToolDependencyEngine.GetAllPackages();

            if (!allPackages.Any(p => p.PendingInstallState))
            {
                e.Cancel = true;
                await MessageManager.ShowOk(Lang.NoPackagesSelected, Lang.YouMustInstallAtLeastOnePackageToContinue);
                return;
            }

            var toInstall = allPackages.Where(p => p.PendingInstallState && !p.IsInstalled).ToList();
            var toUninstall = allPackages.Where(p => !p.PendingInstallState && p.IsInstalled).ToList();

            if (toInstall.Count == 0 && toUninstall.Count == 0)
            {
                _isSafeToClose = true;
                return;
            }

            e.Cancel = true;
            this.IsEnabled = false;

            var controller = await this.ShowProgressAsync(Lang.UpdatingPackages, Lang.PleaseWaitWhilePackagesAreBeingUpdated);
            controller.SetIndeterminate();

            try
            {
                foreach (var pkg in toUninstall)
                {
                    ToolDependencyEngine.UninstallDependency(pkg.ToolType.ToLower(), pkg.ExecutableName);
                }

                foreach (var pkg in toInstall)
                {
                    await ToolDependencyEngine.DependenciesManagerAsync(pkg.DownloadUrl, pkg.ToolType.ToLower());
                }
            }
            finally
            {
                await controller.CloseAsync();
                _isSafeToClose = true;
                this.IsEnabled = true;
                this.Close();
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(AppConfig.PakMasterConfigsFolder))
                {
                    Directory.CreateDirectory(AppConfig.PakMasterConfigsFolder);
                }

                string firstLaunchFile = Path.Combine(AppConfig.PakMasterConfigsFolder, "firstlaunch.complete");
                if (!File.Exists(firstLaunchFile))
                {
                    File.Create(firstLaunchFile).Dispose();
                }
            }
            catch
            {
            }
        }
    }
}