using System.Windows;
using System.Windows.Controls;

namespace Auremo.Controls.CustomWidgets
{
    public class StatefulButton : Button
    {
        public bool IsPushed
        {
            get
            {
                return (bool)GetValue(IsPushedProperty);
            }
            set
            {
                SetValue(IsPushedProperty, value);
            }
        }

        public static readonly DependencyProperty IsPushedProperty = 
            DependencyProperty.Register("IsPushed", typeof(bool), typeof(StatefulButton), new UIPropertyMetadata(false));
    }
}
