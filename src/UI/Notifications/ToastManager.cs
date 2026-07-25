namespace PakMaster.UI.Notifications
{
    public class ToastManager : IToastManager
    {
        private readonly INotificationManager _notificationManager;

        public ToastManager()
        {
            _notificationManager = new NotificationManager();
        }

        // Default Toasts
        public Task ShowInfo(string title, string message, int durationSeconds = 3) =>
            SendToast(title, message, NotificationType.Information, durationSeconds);

        public Task ShowSuccess(string title, string message, int durationSeconds = 3) =>
            SendToast(title, message, NotificationType.Success, durationSeconds);

        public Task ShowWarning(string title, string message, int durationSeconds = 3) =>
            SendToast(title, message, NotificationType.Warning, durationSeconds);

        public Task ShowError(string title, string message, int durationSeconds = 3) =>
            SendToast(title, message, NotificationType.Error, durationSeconds);

        // Additional Toasts
        public Task ShowConfigSaved()
        {
            return SendToast(Lang.Toast_Title_ConfigurationSaved, Lang.Toast_Desc_ConfigurationSaved, NotificationType.Success, 3);
        }

        // Send Toast Notification
        private async Task SendToast(string title, string message, NotificationType toastType, int seconds)
        {
            try
            {
                await Application.Current.Dispatcher.Invoke(() =>
                {
                    var builder = NotificationBuilder.Create(title, message)
                        .ExpiresInSeconds(seconds);

                    switch (toastType)
                    {
                        case NotificationType.Success:
                            builder.AsSuccess().WithBackground(NotificationColor.FromHex(ToastConfig.SuccessBackground));
                            break;

                        case NotificationType.Information:
                            builder.AsInformation().WithBackground(NotificationColor.FromHex(ToastConfig.InformationBackground));
                            break;

                        case NotificationType.Warning:
                            builder.AsWarning().WithBackground(NotificationColor.FromHex(ToastConfig.WarningBackground));
                            break;

                        case NotificationType.Error:
                            builder.AsError().WithBackground(NotificationColor.FromHex(ToastConfig.ErrorBackground));
                            break;
                    }

                    GLogger.Here().Debug("Dispatched toast notification. Type: {ToastType} | Title: '{ToastTitle}' | Expiration: '{Duration}s'", toastType, title, seconds);

                    return _notificationManager.ShowAsync(builder.Build());
                });
            }
            catch (Exception ex)
            {
                GLogger.Here().Error(ex, "Failed to render toast for title: '{ToastTitle}'", title);
            }
        }
    }
}