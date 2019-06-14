using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Game2048
{
    public class TileToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = (int)value;
            if (x > 4096)
                x = 4096;
            if (x < 2)
                return null;
            return $"{PathToFiles.Path}{MainVm.SelectedSet}/{x}.gif";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
