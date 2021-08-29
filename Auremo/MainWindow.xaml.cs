using Auremo.Controls;
using Auremo.DataModel;
using System.ComponentModel;
using System.Windows;

namespace Auremo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = World.Create();
            InitializeComponent();
        }

        public PopupOverlay PopupOverlay => m_PopupOverlay;

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Hide immediately as the takedown might take a few seconds.
            Hide();
            World.Destroy();
        }
    }
}
