using Auremo.DataModel.AudioObjects;
using System.Windows;
using System.Windows.Controls;

namespace Auremo.Controls.Utility
{
    public static class WidgetUtility
    {
        public static T FindAncestor<T>(FrameworkElement element) where T : FrameworkElement
        {
            FrameworkElement elem = element;

            while (elem != null)
            {
                if (elem is T result)
                {
                    return result;
                }
                else if (elem.Parent is FrameworkElement parent)
                {
                    elem = parent;
                }
                else if (elem.TemplatedParent is FrameworkElement templatedParent && templatedParent != null)
                {
                    elem = templatedParent;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public static T FindDataObject<T>(FrameworkElement element) where T : AudioObject
        {
            FrameworkElement elem = element;

            while (elem != null)
            {
                if (elem.DataContext is T obj)
                {
                    return obj;
                }
                else if (elem.Parent is FrameworkElement parent && parent != null)
                {
                    elem = parent;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }
    }
}
