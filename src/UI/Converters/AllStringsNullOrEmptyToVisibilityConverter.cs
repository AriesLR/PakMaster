namespace PakMaster.UI.Converters
{
    public class AllStringsNullOrEmptyToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool allEmpty = values.All(v => string.IsNullOrWhiteSpace(v as string));

            return allEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}