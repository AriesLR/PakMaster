namespace PakMaster.UI.Views
{
    public partial class TemplateView : UserControl
    {
        public TemplateView()
        {
            InitializeComponent();
        }

        // ============ Event Handlers ============

        // Hyperlink Click
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            UrlOperations.OpenUrlAsync(e.Uri.AbsoluteUri);

            e.Handled = true;
        }

        // ============ Debug/Testing Section ============

        // Toast Service Full Test Button
        private async void ToastServiceFullTest_Click(object sender, RoutedEventArgs e)
        {
            _ = App.Toasts.ShowInfo("Toast Service Test (Info)", "This is a test of the toast notification service.", 4);
            await Task.Delay(500);
            _ = App.Toasts.ShowSuccess("Toast Service Test (Success)", "This is a test of the toast notification service.", 4);
            await Task.Delay(500);
            _ = App.Toasts.ShowWarning("Toast Service Test (Warning)", "This is a test of the toast notification service.", 4);
            await Task.Delay(500);
            _ = App.Toasts.ShowError("Toast Service Test (Error)", "This is a test of the toast notification service.", 4);

            GLogger.Here().Debug("Toast Service test has been completed.");
        }

        // Toast Service Info Test Button
        private async void ToastServiceInfoTest_Click(object sender, RoutedEventArgs e)
        {
            await App.Toasts.ShowInfo("Toast Service Test (Info)", "This is a test of the toast notification service.", 4);

            GLogger.Here().Debug("Toast Service test (Info) has been completed.");
        }

        // Toast Service Success Test Button
        private async void ToastServiceSuccessTest_Click(object sender, RoutedEventArgs e)
        {
            await App.Toasts.ShowSuccess("Toast Service Test (Success)", "This is a test of the toast notification service.", 4);

            GLogger.Here().Debug("Toast Service test (Success) has been completed.");
        }

        // Toast Service Warning Test Button
        private async void ToastServiceWarningTest_Click(object sender, RoutedEventArgs e)
        {
            await App.Toasts.ShowWarning("Toast Service Test (Warning)", "This is a test of the toast notification service.", 4);

            GLogger.Here().Debug("Toast Service test (Warning) has been completed.");
        }

        // Toast Service Error Test Button
        private async void ToastServiceErrorTest_Click(object sender, RoutedEventArgs e)
        {
            await App.Toasts.ShowError("Toast Service Test (Error)", "This is a test of the toast notification service.", 4);

            GLogger.Here().Debug("Toast Service test (Error) has been completed.");
        }

        // Test UI Exception Button
        public void TestUIException_Click(object sender, RoutedEventArgs e)
        {
            throw new InvalidOperationException("Testing UI thread exception handling.");
        }

        // Test Unobserved Task Exception Button
        public void TestUnobservedTaskException_Click(object sender, RoutedEventArgs e)
        {
            GenerateUnobservedTask();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        // Test Unobserved Task Helper
        private static void GenerateUnobservedTask()
        {
            Task.Run(() => throw new InvalidOperationException("Testing unobserved task exception."));
        }

        // Test Fatal App Domain Exception Button
        public void TestFatalAppDomainException_Click(object sender, RoutedEventArgs e)
        {
            var crashThread = new System.Threading.Thread(() =>
            {
                throw new InvalidOperationException("Testing non-UI thread exception.");
            });

            crashThread.Start();
        }

        // Message Service Info Test Button
        private async void TestShowInfo_Click(object sender, RoutedEventArgs e)
        {
            await MessageManager.ShowInfo("Message Service Test", "(Info)\nThis is a test of the message service.");

            GLogger.Here().Debug("Message Service test (Info) has been completed.");
        }

        // Message Service Warning Test Button
        private async void TestShowWarning_Click(object sender, RoutedEventArgs e)
        {
            await MessageManager.ShowWarning("(Warning)\nThis is a test of the message service.");

            GLogger.Here().Debug("Message Service test (Warning) has been completed.");
        }

        // Message Service Error Test Button
        private async void TestShowError_Click(object sender, RoutedEventArgs e)
        {
            await MessageManager.ShowError("(Error)\nThis is a test of the message service.");

            GLogger.Here().Debug("Message Service test (Error) has been completed.");
        }

        // MessageBox Test Button
        private void TestMessageBox_Click(object sender, RoutedEventArgs e)
        {
            string logFilePath = "C:\\Path\\To\\Log\\File\\Could\\Look\\Like\\This\\If\\You\\Were\\A\\Crazy\\Person\\debug.log";
            string exMessage = "Dummy exception message.";
            string errorMessage = string.Format(Lang.CrashReportWindow_Msg_SubmitError_Desc, logFilePath, exMessage);

            MessageBox.Show(errorMessage, Lang.Msg_Error_Title, MessageBoxButton.OK, MessageBoxImage.Error);

            GLogger.Here().Debug("MessageBox test has been completed.");
        }
    }
}