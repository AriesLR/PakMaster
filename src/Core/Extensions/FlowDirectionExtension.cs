namespace PakMaster.Core.Extensions
{
    public class FlowDirectionExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var initialDir = LanguageManager.CurrentFlowDirection;
            var initialCulture = CultureInfo.CurrentUICulture.Name;

            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target
                && target.TargetObject is FrameworkElement element)
            {
                EventHandler languageHandler = (s, e) =>
                {
                    element.Dispatcher.InvokeAsync(() =>
                    {
                        var newDir = LanguageManager.CurrentFlowDirection;
                        element.FlowDirection = newDir;
                    });
                };

                LanguageManager.LanguageChanged += languageHandler;

                RoutedEventHandler loadedHandler = null!;
                loadedHandler = (s, e) =>
                {
                    var loadedDir = LanguageManager.CurrentFlowDirection;
                    element.FlowDirection = loadedDir;
                    element.Loaded -= loadedHandler;
                };
                element.Loaded += loadedHandler;

                element.Unloaded += (s, e) =>
                {
                    LanguageManager.LanguageChanged -= languageHandler;
                };
            }

            return initialDir;
        }
    }
}