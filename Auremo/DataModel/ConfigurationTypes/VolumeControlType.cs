using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Auremo.DataModel.Types
{
    public enum VolumeControlType
    {
        None = 0,
        Wheel = 1,
        Slider = 2,
        Buttons = 3
    }

    [ValueConversion(typeof(VolumeControlType), typeof(string))]
    public class VolumeControlTypeTranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: take value.ToString(), prefix it with something
            // and pass it to the translation dictionary or something.
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("VolumeControlTypeTranslationConverter.ConvertBack(): logic error.");
        }
    }
}
