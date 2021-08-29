using Auremo.DataModel;
using Auremo.DataModel.Types;
using Auremo.Network;
using System.Windows;
using System.Windows.Controls;

namespace Auremo.Controls
{
    public partial class NavigationPanel : UserControl
    {
        public NavigationPanel()
        {
            InitializeComponent();
        }

        private void OnActivityClick(object sender, RoutedEventArgs e)
        {
            World.Instance.InterfaceState.DisplayView(ViewMode.Activity);
        }

        private void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            #warning TODO not sure, maybe either allow null search in QueryResult DisplayView or make browse unclickable (why not?)
            World.Instance.InterfaceState.DisplayView(ViewMode.QueryResult);
        }

        private void OnFilesClick(object sender, RoutedEventArgs e)
        {
            World.Instance.QueryResult.DisplayView(new Command("lsinfo"));
        }

        private void OnArtistsClick(object sender, RoutedEventArgs e)
        {
            World.Instance.QueryResult.DisplayView(new Command("list", "artist"));
        }

        private void OnGenresClick(object sender, RoutedEventArgs e)
        {
            World.Instance.QueryResult.DisplayView(new Command("list", "genre"));
        }

        private void OnPlaylistsClick(object sender, RoutedEventArgs e)
        {
            World.Instance.InterfaceState.DisplayView(ViewMode.PlaylistSelector);
        }

        private void OnPlayQueueClick(object sender, RoutedEventArgs e)
        {
            World.Instance.InterfaceState.DisplayView(ViewMode.PlayQueue);
        }

        private void OnServerClick(object sender, RoutedEventArgs e)
        {
            World.Instance.InterfaceState.DisplayView(ViewMode.Server);
        }

        private void OnConfigurationClick(object sender, RoutedEventArgs e)
        {
            World.Instance.InterfaceState.DisplayView(ViewMode.Configuration);
        }

        private void OnDeverloperClick(object sender, RoutedEventArgs e)
        {
            World.Instance.InterfaceState.DisplayView(ViewMode.Developer);
        }
    }
}
