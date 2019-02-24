using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static System.String;

namespace TimeSeriesAnalyzer.ViewModel.Converters {
    internal class PointToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Point p)
                return $"({p.X:0.###}; {p.Y:0.###})";

            return Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}