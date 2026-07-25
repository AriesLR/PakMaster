using PakMaster.Core.Extensions;

namespace PakMaster.UI.Notifications
{
    public static class MessageManager
    {
        private static MetroWindow GetMainWindow()
        {
            return (Application.Current.MainWindow as MetroWindow)!;
        }

        // ============ Basic Dialogs ============

        // Info
        public static async Task ShowInfo(string title, string message)
        {
            GLogger.Here().Debug("Displaying Info Dialog. Title: '{Title}', Message: '{Message}'", title, message);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Ok
            };
            await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, settings));
        }

        // Warning
        public static async Task ShowWarning(string message)
        {
            GLogger.Here().Warning("Displaying User Warning Dialog. Message: '{Message}'", message);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Ok
            };
            await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowMessageAsync("Warning", message, MessageDialogStyle.Affirmative, settings));
        }

        // Error
        public static async Task ShowError(string message)
        {
            GLogger.Here().Error("Displaying User Error Dialog. Message: '{Message}'", message);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Ok
            };
            await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowMessageAsync("Error", message, MessageDialogStyle.Affirmative, settings));
        }

        // Show Progress Bar
        public static async Task ShowProgress(string title, string message, Func<IProgress<double>, Task> operation)
        {
            if (Application.Current.MainWindow is not MetroWindow mainWindow)
                throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            GLogger.Here().Debug("Launching Progress Dialog. Title: '{Title}'", title);
            var controller = await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowProgressAsync(title, message));
            await Application.Current.Dispatcher.InvokeSafeAsync(() => controller.SetIndeterminate());

            try
            {
                var progress = new Progress<double>(value => Application.Current.Dispatcher.InvokeSafeAsync(() => controller.SetProgress(value)));
                await operation(progress);
                GLogger.Here().Debug("Progress Dialog task operation completed successfully.");
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "An unhandled exception occurred within the Progress Dialog running operation.");
                throw;
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeSafeAsync(() => controller.CloseAsync());
            }
        }

        // ============ Confirm/Deny Dialogs ============

        // Yes/No
        public static async Task<bool> ShowYesNo(string title, string message)
        {
            GLogger.Here().Debug("Displaying Yes/No Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Yes,
                NegativeButtonText = Lang.Btn_No
            };

            var result = await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                settings
            ));

            bool userConfirmed = result == MessageDialogResult.Affirmative;
            GLogger.Here().Debug("Yes/No Prompt returned. User selected: {UserChoice}", userConfirmed ? "Yes" : "No");
            return userConfirmed;
        }

        // Yes/Cancel
        public static async Task<bool> ShowYesCancel(string title, string message)
        {
            GLogger.Here().Debug("Displaying Yes/Cancel Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Yes,
                NegativeButtonText = Lang.Btn_Cancel
            };

            var result = await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                settings
            ));

            bool userConfirmed = result == MessageDialogResult.Affirmative;
            GLogger.Here().Debug("Yes/Cancel Prompt returned. User selected: {UserChoice}", userConfirmed ? "Yes" : "Cancel");
            return userConfirmed;
        }

        // ============ Confirmation Dialogs ============

        // Ok
        public static async Task<bool> ShowOk(string title, string message)
        {
            GLogger.Here().Debug("Displaying Ok Confirmation Dialog. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");

            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Ok
            };

            var result = await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.Affirmative,
                settings
            ));

            return result == MessageDialogResult.Affirmative;
        }

        // ============ Special Dialogs ============

        // TextBox Input
        public static async Task<string> ShowInput(string title, string message)
        {
            GLogger.Here().Debug("Displaying Text Input Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Ok,
                NegativeButtonText = Lang.Btn_Cancel,
                DefaultText = "",
                AnimateShow = true,
                AnimateHide = true
            };

            var result = await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowInputAsync(title, message, settings));

            if (result == null)
            {
                GLogger.Here().Debug("Text Input Prompt canceled by user.");
            }
            else
            {
                GLogger.Here().Debug("Text Input Prompt submitted. Received input length: {InputLength} characters.", result.Length);
            }

            return result ?? string.Empty;
        }

        // Folder Browser Prompt
        public static async Task<bool> ShowBrowseCancel(string title, string message)
        {
            GLogger.Here().Debug("Displaying Browse/Cancel Prompt. Title: '{Title}'", title);
            var mainWindow = GetMainWindow() ?? throw new InvalidOperationException("Main window is not a MetroWindow or has not been set.");
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = Lang.Btn_Browse,
                NegativeButtonText = Lang.Btn_Cancel
            };

            var result = await Application.Current.Dispatcher.InvokeSafeAsync(() => mainWindow.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                settings
            ));

            bool userClickedBrowse = result == MessageDialogResult.Affirmative;
            GLogger.Here().Debug("Browse/Cancel Prompt returned. User selected: {UserChoice}", userClickedBrowse ? "Browse" : "Cancel");
            return userClickedBrowse;
        }
    }
}