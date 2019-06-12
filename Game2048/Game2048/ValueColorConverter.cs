using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Game2048
{
    public class ValueColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (int)value;
            switch (v)
            {
                case 0: return Color.LightGray;
                case 2: return Color.Beige;
                case 4: return Color.AntiqueWhite;
                case 8: return Color.Yellow;
                case 16: return Color.Gold;
                case 32: return Color.Wheat;
                case 64: return Color.Azure;
                case 128: return Color.AliceBlue;
                case 256: return Color.Coral;
                case 512: return Color.SkyBlue;
                case 1024: return Color.Lavender;
                case 2048: return Color.Goldenrod;
                default:
                    return Color.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
