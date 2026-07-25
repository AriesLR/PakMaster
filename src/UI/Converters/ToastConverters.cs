namespace PakMaster.UI.Converters
{
    public class ToastTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotificationType type)
            {
                return type switch
                {
                    NotificationType.Success => ToastConfig.SuccessBackgroundBrush,
                    NotificationType.Information => ToastConfig.InformationBackgroundBrush,
                    NotificationType.Warning => ToastConfig.WarningBackgroundBrush,
                    NotificationType.Error => ToastConfig.ErrorBackgroundBrush,
                    _ => ToastConfig.InformationBackgroundBrush
                };
            }
            return ToastConfig.InformationBackgroundBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class ToastTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotificationType type)
            {
                return type switch
                {
                    NotificationType.Success => PackIconMaterialKind.CheckCircleOutline,
                    NotificationType.Information => PackIconMaterialKind.InformationOutline,
                    NotificationType.Warning => PackIconMaterialKind.AlertOutline,
                    NotificationType.Error => PackIconMaterialKind.AlertOctagonOutline,
                    _ => PackIconMaterialKind.InformationOutline
                };
            }
            return PackIconMaterialKind.InformationOutline;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
