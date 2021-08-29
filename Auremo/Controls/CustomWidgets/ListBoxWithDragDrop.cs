using System.Windows;
using System.Windows.Controls;

namespace Auremo.Controls.CustomWidgets
{
    public class ListBoxWithDragDrop : ListBox
    {
        public bool IsDragActive
        {
            get
            {
                return (bool)GetValue(IsDragActiveProperty);
            }
            set
            {
                SetValue(IsDragActiveProperty, value);
            }
        }

        public static readonly DependencyProperty IsDragActiveProperty = DependencyProperty.Register("IsDragActive", typeof(bool), typeof(ListBoxWithDragDrop), new UIPropertyMetadata(false));
    }
}
