using Auremo.DataModel.Types;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Auremo.Controls.Utility
{
    [ValueConversion(typeof(double), typeof(double))]
    public class RoundDoubleConverter : IValueConverter
    {
        // Converts the value to the nearest multiple of double argument parameter.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;
            double resolution = (double)parameter;
            return Math.Round(val / resolution, MidpointRounding.AwayFromZero) * resolution;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class EnumEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: This is a really cheesy way of accomplishing this, but I can't think of anything
            // else that works. This is meant to compare two enums of the same type, without
            // having to specify a dedicated converter to each enum type, of which there are a few.)
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == parameter;
        }
    }

    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }

    [ValueConversion(typeof(ViewMode), typeof(bool))]
    class ViewModeEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ViewMode lhs && parameter is ViewMode rhs)
            {
                return rhs == lhs;
            }
            else
            {
                throw new Exception("ViewModeEqualsConverter: logic error.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    class VolumeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return "VOL: " + i.ToString();
            }
            else
            {
                throw new Exception("VolumeStringConverter: logic error.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(double?), typeof(string))]
    public class TimecodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "-:--:--";
            }
            else if (value is double val)
            {
                int secs = (int)val;
                int hh = secs / 3600;
                int mm = (secs % 3600) / 60;
                int ss = secs % 60;

                if (hh == 0)
                {
                    return string.Format("{0,2:00}:{1,2:00}", mm, ss);
                }
                else
                {
                    return string.Format("{0}:{1,2:00}:{2,2:00}", hh, mm, ss);
                }
            }
            else
            {
                throw new Exception("TimecodeToStringConverter: logic error.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // This cleans up nulls from time codes, making them safe for a slider's
    // Maximum property. It could instead be done using a FallbackValue,
    // but that throws exceptions that fill up the debug output window.
    [ValueConversion(typeof(double?), typeof(double))]
    public class TimecodeToSliderMaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double?)value).GetValueOrDefault(1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
