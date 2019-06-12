using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Game2048
{
    public class ArrayPicker : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var arr = (TileVm[,])value;
            var indices = parameter.ToString().Split('-');
            var index0 = int.Parse(indices[0]);
            var index1 = int.Parse(indices[1]);
            return arr[index0, index1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
