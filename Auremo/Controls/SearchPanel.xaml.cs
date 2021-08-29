using Auremo.DataModel;
using Auremo.DataModel.Types;
using Auremo.Network;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Auremo.Controls
{
    public partial class SearchPanel : UserControl, INotifyPropertyChanged
    {
        public SearchPanel()
        {
            InitializeComponent();
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            string search = World.Instance.InterfaceState.SearchString;

            if (search.Length > 1)
            {
                World.Instance.QueryResult.DisplayView(new Command("search", $"(any contains '{search}')"));
            }
        }

        private void GoToStart(object sender, RoutedEventArgs e)
        {
            World.Instance.ViewHistory.Restart();
        }

        public void GoBack(object sender, RoutedEventArgs e)
        {
            World.Instance.ViewHistory.GoBack();
        }

        public void GoForward(object sender, RoutedEventArgs e)
        {
            World.Instance.ViewHistory.GoForward();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
