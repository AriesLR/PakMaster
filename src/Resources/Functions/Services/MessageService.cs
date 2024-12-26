using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace PakMaster.Resources.Functions.Services
{
    public static class MessageService
    {
        private static MetroWindow GetMainWindow()
        {
            return Application.Current.MainWindow as MetroWindow;
        }

        public static async Task ShowProgress(string title, string message, Func<IProgress<double>, Task> operation)
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;
            if (mainWindow == null)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            var controller = await mainWindow.ShowProgressAsync(title, message);
            controller.SetIndeterminate();

            try
            {
                var progress = new Progress<double>(value => controller.SetProgress(value));
                await operation(progress);
            }
            finally
            {
                await controller.CloseAsync();
            }
        }

        public static async Task<bool> ShowYesNo(string title, string message)
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No"
            };

            var result = await mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                settings
            );

            return result == MessageDialogResult.Affirmative;
        }

        public static async Task ShowInfo(string title, string message)
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            await mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative);
        }
        public static async Task ShowWarning(string message)
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            await mainWindow.ShowMessageAsync("Warning", message, MessageDialogStyle.Affirmative);
        }

        public static async Task ShowError(string message)
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            await mainWindow.ShowMessageAsync("Error", message, MessageDialogStyle.Affirmative);
        }
    }
}