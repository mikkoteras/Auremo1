using Auremo.DataModel;
using Auremo.DataModel.AudioObjects;
using Auremo.Network;
using System.Windows;
using System.Windows.Controls;

namespace Auremo.Controls
{
    public partial class PlaylistSelectorView : UserControl
    {
        public PlaylistSelectorView()
        {
            InitializeComponent();
        }

        private void OnPlaylistClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement widget)
            {
                if (widget.DataContext is Playlist playlist)
                {
                    World.Instance.PlaylistInfo.DisplayView(playlist);
                }
            }
        }

        private void OnLoadPlaylistClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Playlist playlist)
            {
                World.Instance.CommandRunner.PushCommand(new Command("load", playlist.File));
            }
        }

        private void OnSaveQueueAsClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).PopupOverlay.Ask(
                World.Instance.Translator.FixMeMissingTranslation("Name for the new playlist:"),
                (bool ok, string name) =>
                {
                    if (ok)
                    {
                        World.Instance.CommandRunner.PushCommand(new Command("save", name));
                    }
                });
        }

        private void OnRenamePlaylistClicked(object sender, RoutedEventArgs e)
        {
            if (m_Items.SelectedItem != null && m_Items.SelectedItem is Playlist playlist)
            {
                ((MainWindow)Application.Current.MainWindow).PopupOverlay.Ask(
                    World.Instance.Translator.FixMeMissingTranslation("Name for the new playlist:"),
                    (bool ok, string name) =>
                    {
                        if (ok)
                        {
                            World.Instance.CommandRunner.PushCommand(new Command("rename", playlist.File, name));
                        }
                    });
            }
        }

        private void OnRemovePlaylistClicked(object sender, RoutedEventArgs e)
        {
            #warning TODO should we ask for confirmation? Requires changes to the overlay, buuuuuuut probably yes...
            if (m_Items.SelectedItem != null && m_Items.SelectedItem is Playlist playlist)
            {
                World.Instance.CommandRunner.PushCommand(new Command("rm", playlist.File));
            }
        }

        private void OnLoadSelectedPlaylistClicked(object sender, RoutedEventArgs e)
        {
            if (m_Items.SelectedItem != null && m_Items.SelectedItem is Playlist playlist)
            {
                World.Instance.CommandRunner.PushCommand(new Command("load", playlist.File));
            }
        }
    }
}
