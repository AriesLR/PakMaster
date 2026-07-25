namespace PakMaster.UI.Notifications.Interfaces
{
    public interface IToastManager
    {
        Task ShowInfo(string title, string message, int durationSeconds = 3);

        Task ShowSuccess(string title, string message, int durationSeconds = 3);

        Task ShowWarning(string title, string message, int durationSeconds = 3);

        Task ShowError(string title, string message, int durationSeconds = 3);

        Task ShowConfigSaved();
    }
}