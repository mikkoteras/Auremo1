using Auremo.DataModel;
using System.Windows.Controls;

namespace Auremo.Controls
{
    public partial class DeveloperView : UserControl
    {
        public DeveloperView()
        {
            InitializeComponent();
        }

        private void OnClearLogClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            World.Instance.Log.Clear();
        }

        private void OnSendClick(object sender, System.Windows.RoutedEventArgs e)
        {
            World.Instance.DeveloperInfo.SendRequest();
        }
    }
}
