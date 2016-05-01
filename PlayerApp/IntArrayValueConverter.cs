using System;
using System.Globalization;
using Community.Windows.ValueConverters;
using System.Linq;
using System.Windows.Data;

namespace PlayerApp
{
    [ValueConversion(typeof(int[]), typeof(String))]
    public class IntArrayConverter : ConverterBase {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value != null && value.GetType() == typeof(int[])) {
                var arr = value as int[];
                if(arr != null && arr.Length > 0) {
                    return "[" + String.Join(", \n", arr.Select( (i) => i.ToString() ).ToArray() ) + "]";
                }
            }
            return "";
        }
       
    }
}
