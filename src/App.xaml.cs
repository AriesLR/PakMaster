using PakMaster.Resources.Functions.Services;
using System.Windows;

namespace PakMaster
{
    public partial class App : Application
    {
        private async void AppStartup(object sender, StartupEventArgs e)
        {
            // Check if repak.exe exists in bin/repak
            if (!DependenciesService.CheckIfDependencyExists("repak", "repak.exe"))
            {
                await DependenciesService.DependenciesManagerAsync("https://github.com/trumank/repak/releases/download/v0.2.2/repak_cli-x86_64-pc-windows-msvc.zip", "repak");
            }
            else
            {
                Console.WriteLine("repak.exe already exists.");
            }

            // Check if zentools.exe exists in bin/zentools
            if (!DependenciesService.CheckIfDependencyExists("zentools", "ZenTools.exe"))
            {
                await DependenciesService.DependenciesManagerAsync("https://github.com/LongerWarrior/ZenTools/releases/download/1.06UE5.1-5.2/ZenTools.exe", "zentools");
            }
            else
            {
                Console.WriteLine("ZenTools.exe already exists.");
            }
        }
    }

}
