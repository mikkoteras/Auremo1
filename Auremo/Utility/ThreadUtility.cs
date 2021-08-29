using System;
using System.Windows;
using System.Windows.Threading;

namespace Auremo.Controls.Utility
{
    public static class ThreadUtility
    {
        
        public static void RunInUiThread(Action action)
        {
            // Impose a 2 second timeout, because when the window is closing,
            // Invoke() will not return and the application hangs.
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                                                  TimeSpan.FromSeconds(2),
                                                  action);
        }
    }
}
