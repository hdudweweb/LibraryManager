using System;
using System.Globalization;
using System.Windows.Data;
using WpfApp12.Models;

namespace WpfApp12.Views
{
    /// <summary>BookStatus → русский текст для отображения.</summary>
    public class BookStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BookStatus status)
                return status == BookStatus.Available ? "В наличии" : "Выдана";
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
