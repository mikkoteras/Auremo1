using Auremo.DataModel;
using Auremo.DataModel.AudioObjects;
using Auremo.Network;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls
{
    public partial class QueryResultView : UserControl, INotifyPropertyChanged
    {
        public QueryResultView()
        {
            InitializeComponent();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // TODO
            }
        }

        private void OnDirectoryClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement widget)
            {
                if (widget.DataContext is Directory directory)
                {
                    World.Instance.QueryResult.DisplayView(new Command("lsinfo", directory.Name));
                }
            }
        }

        private void OnArtistClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement widget)
            {
                if (widget.DataContext is Artist artist)
                {
                    World.Instance.QueryResult.DisplayView(new Command("find", $"(artist == '{artist.Name}')"));
                }
                else if (widget.DataContext is Track track)
                {
                    World.Instance.QueryResult.DisplayView(new Command("find", $"(artist == '{track.Artist}')"));
                }
            }
        }

        private void OnGenreClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement widget)
            {
                if (widget.DataContext is Genre genre)
                {
                    World.Instance.QueryResult.DisplayView(new Command("find", $"(genre == '{genre.Name}')"));
                }
            }
        }

        private void OnAlbumClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement widget)
            {
                if (widget.DataContext is Track track)
                {
                    World.Instance.QueryResult.DisplayView(new Command("find", $"((artist == '{track.Artist}') AND (album == '{track.Album}'))"));
                }
            }
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

        private void OnAddItemClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is AudioObject o)
            {
                World.Instance.CommandRunner.PushCommand(o.CommandToAdd);
            }
        }

        private void OnPlayItemClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is AudioObject o)
            {
                World.Instance.CommandRunner.PushCommand(new CommandList(
                    new Command("clear"),
                    o.CommandToAdd,
                    new Command("play", "0")));
            }
        }

        private void OnAddSelectionToPlaylistClicked(object sender, RoutedEventArgs e)
        {
            ((Button)sender).ContextMenu.IsOpen = true;
            return;
        }

        private void OnPlaySelectionClicked(object sender, RoutedEventArgs e)
        {
            CommandList list = new CommandList();
            list.Commands.Add(new Command("clear"));

            foreach (object sel in m_QueryResult.SelectedItems)
            {
                if (sel is AudioObject o)
                {
                    list.Commands.Add(o.CommandToAdd);
                }
            }

            list.Commands.Add(new Command("play"));
            World.Instance.CommandRunner.PushCommand(list);
        }

        private void OnAddSelectionClicked(object sender, RoutedEventArgs e)
        {
            CommandList list = new CommandList();

            foreach (object sel in m_QueryResult.SelectedItems)
            {
                if (sel is AudioObject o)
                {
                    list.Commands.Add(o.CommandToAdd);
                }
            }

            World.Instance.CommandRunner.PushCommand(list);
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
