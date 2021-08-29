using Auremo.Controls.Utility;
using Auremo.DataModel;
using Auremo.DataModel.AudioObjects;
using Auremo.Network;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls
{
    public partial class PlaylistView : UserControl
    {
        private readonly DragAndDropManager m_DragAndDropManager = null;

        public PlaylistView()
        {
            InitializeComponent();

            // World.Instance is guaranteed to exist, except for the Designer.
            if (World.Instance != null)
            {
                m_DragAndDropManager = new DragAndDropManager(m_ItemList,
                                                              World.Instance.PlaylistInfo.SelectedPlaylistContents,
                                                              OnDrop);
            }
        }

        private void OnArtistClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlaylistItem item)
            {
                World.Instance.QueryResult.DisplayView(new Command("find", $"(artist == '{item.Artist}')"));
            }
        }

        private void OnAlbumClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlaylistItem item)
            {
                World.Instance.QueryResult.DisplayView(new Command("find", $"((artist == '{item.Artist}') AND (album == '{item.Album}'))"));
            }
        }

        private void OnPlayItemClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlaylistItem item)
            {
                World.Instance.CommandRunner.PushCommand(new CommandList(
                    new Command("clear"),
                    item.CommandToAdd,
                    new Command("play", "0")));
            }
        }

        private void OnAddItemClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlaylistItem item)
            {
                World.Instance.CommandRunner.PushCommand(item.CommandToAdd);
            }
        }

        private void OnRemoveItemClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlaylistItem item)
            {
                string playlist = World.Instance.PlaylistInfo.SelectedPlaylist.File;
                World.Instance.CommandRunner.PushCommand(new Command("playlistdelete", playlist, item.Pos));
            }
        }

        private void OnClearPlaylistClick(object sender, RoutedEventArgs e)
        {
            string playlist = World.Instance.PlaylistInfo.SelectedPlaylist.File;
            World.Instance.CommandRunner.PushCommand(new Command("playlistclear", playlist));
        }

        private void OnAddUrlToPlaylistClick(object sender, RoutedEventArgs e)
        {
            string playlist = World.Instance.PlaylistInfo.SelectedPlaylist.File;
            ((MainWindow)Application.Current.MainWindow).PopupOverlay.Ask(
                World.Instance.Translator.FixMeMissingTranslation("Add a URL to the playlist:"),
                (bool ok, string url) =>
                {
                    if (ok)
                    {
                        World.Instance.CommandRunner.PushCommand(new Command("playlistadd", playlist, url));
                    }
                });
        }

        private void OnDeleteItemsClicked(object sender, RoutedEventArgs e)
        {
            string playlist = World.Instance.PlaylistInfo.SelectedPlaylist.File;
            CommandList list = new CommandList();
            int positionCorrection = 0; // Because positions change with each deleted item.

            foreach (object o in m_ItemList.SelectedItems)
            {
                if (o is PlaylistItem item)
                {
                    list.Commands.Add(new Command("playlistdelete", playlist, item.Pos - positionCorrection));
                    positionCorrection += 1;
                }
            }

            World.Instance.CommandRunner.PushCommand(list);
        }

        private void OnLoadToQueueClick(object sender, RoutedEventArgs e)
        {
            string playlist = World.Instance.PlaylistInfo.SelectedPlaylist.File;
            World.Instance.CommandRunner.PushCommand(new Command("load", playlist));

        }

        #region Drag and drop

        private void OnPreviewLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_DragAndDropManager.OnPreviewLeftMouseDown(e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            m_DragAndDropManager.OnMouseMove(e);
        }

        private void OnPreviewLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            m_DragAndDropManager.OnPreviewLeftMouseUp();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            m_DragAndDropManager.OnMouseLeave();
        }

        public void OnDrop(int position)
        {
            IList<PlaylistItem> items = World.Instance.PlaylistInfo.SelectedPlaylistContents;

            if (position <= items.Count) // Probably redundant, but safe.
            {
                int targetPosition = position;
                CommandList list = new CommandList();
                string playlist = World.Instance.PlaylistInfo.SelectedPlaylist.File;
                int positionCorrection = 0; // How the target position changes due to items moving around

                foreach (object o in m_ItemList.SelectedItems)
                {
                    PlaylistItem item = o as PlaylistItem;
                    DependencyObject row = m_ItemList.ItemContainerGenerator.ContainerFromItem(item);
                    int itemIndex = m_ItemList.ItemContainerGenerator.IndexFromContainer(row) + positionCorrection;

                    if (itemIndex < targetPosition)
                    {
                        list.Commands.Add(new Command("playlistmove", playlist, itemIndex, targetPosition - 1));
                        positionCorrection -= 1;
                    }
                    else
                    {
                        list.Commands.Add(new Command("playlistmove", playlist, itemIndex, targetPosition++));
                    }
                }

                World.Instance.CommandRunner.PushCommand(list);
            }
        }

        #endregion
    }
}
