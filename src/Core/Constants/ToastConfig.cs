namespace PakMaster.Core.Constants
{
    public static class ToastConfig
    {
        // Toast Background Colors
        public static readonly string SuccessBackground = "#43A047";

        public static readonly string InformationBackground = "#039BE5";

        public static readonly string WarningBackground = "#FB8C00";

        public static readonly string ErrorBackground = "#E53935";

        // Toast Background Brushes
        public static readonly Brush SuccessBackgroundBrush = CreateFrozenBrush(SuccessBackground);

        public static readonly Brush InformationBackgroundBrush = CreateFrozenBrush(InformationBackground);
        public static readonly Brush WarningBackgroundBrush = CreateFrozenBrush(WarningBackground);
        public static readonly Brush ErrorBackgroundBrush = CreateFrozenBrush(ErrorBackground);

        // Toast Brush Helper
        private static Brush CreateFrozenBrush(string hexColor)
        {
            var color = (Color)ColorConverter.ConvertFromString(hexColor);
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }
}