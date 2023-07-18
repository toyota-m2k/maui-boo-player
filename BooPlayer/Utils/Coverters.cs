using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooPlayer.Utils;
public class TimeSpanDoubleConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is TimeSpan ts) {
            return ts.TotalSeconds;
        }
        else return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        if(value is double d) {
            return TimeSpan.FromSeconds(d);
        }
        else return TimeSpan.Zero;
    }
}
