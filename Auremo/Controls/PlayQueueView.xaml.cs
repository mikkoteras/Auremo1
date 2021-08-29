using Auremo.Controls.Utility;
using Auremo.DataModel;
using Auremo.DataModel.AudioObjects;
using Auremo.Network;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls
{
    public partial class PlayQueueView : UserControl
    {
        private readonly DragAndDropManager m_DragAndDropManager = null;

        public PlayQueueView()
        {
            InitializeComponent();

            // World.Instance is guaranteed to exist, except for the Designer.
            if (World.Instance != null)
            {
                m_DragAndDropManager = new DragAndDropManager(m_ItemList, World.Instance.PlayQueue.Items, OnDrop);
            }
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaySelectedSong();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PlaySelectedSong();
            }
            else if (e.Key == Key.Delete)
            {
                DeleteSelectedSongs();
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

        private void PlaySelectedSong()
        {
            if (m_ItemList.SelectedItems.Count == 1)
            {
                PlaylistItem item = m_ItemList.SelectedItem as PlaylistItem;
                World.Instance.CommandRunner.PushCommand(new Command("play", item.Pos.ToString()));
            }
        }

        private void DeleteSelectedSongs()
        {
            foreach (object o in m_ItemList.SelectedItems)
            {
                PlaylistItem item = o as PlaylistItem;
                World.Instance.CommandRunner.PushCommand(new Command("deleteid", item.Id.ToString()));
            }
        }

        private void OnRemoveItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is PlaylistItem item)
            {
                World.Instance.CommandRunner.PushCommand(new Command("deleteid", item.Id.ToString()));
            }
        }
        private void OnCreatePlaylistFromQueueClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).PopupOverlay.Ask(
                World.Instance.Translator.FixMeMissingTranslation("Create a new playlist with name:"),
                (bool ok, string playlist) =>
                {
                    if (ok)
                    {
                        World.Instance.CommandRunner.PushCommand(new Command("save", playlist));
                    }
                });
        }

        private void OnCreatePlaylistFromSelectionClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            IEnumerable<string> selection = World.Instance.PlayQueue.Items.Where(x => x.IsSelected).Select(x => x.File);

            ((MainWindow)Application.Current.MainWindow).PopupOverlay.Ask(
                World.Instance.Translator.FixMeMissingTranslation("Add item to queue:"),
                (bool ok, string playlist) =>
                {
                    if (ok)
                    {
                        CommandList list = new CommandList();

                        foreach (string item in selection)
                        {
                            list.Commands.Add(new Command("playlistadd", playlist, item));
                        }

                        World.Instance.CommandRunner.PushCommand(list);
                    }
                });
        }

        private void AddSelectionToPlaylistClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: which playlist and where on it?
        }

        private void OnClearPlayQueueClicked(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("clear"));
        }

        private void OnAddUrlClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).PopupOverlay.Ask(
                World.Instance.Translator.FixMeMissingTranslation("Add URL to queue:"),
                (bool ok, string url) =>
                {
                    if (ok)
                    {
                        World.Instance.CommandRunner.PushCommand(new Command("add", url));
                    }
                });
        }

        private void OnShufflePlayQueueClicked(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("shuffle"));
        }

        private void OnSavePlayQueueClicked(object sender, RoutedEventArgs e)
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
            IList<PlaylistItem> items = World.Instance.PlayQueue.Items;

            if (position <= items.Count) // Probably redundant, but safe.
            {
                int targetPosition = position;
                CommandList list = new CommandList();

                foreach (object o in m_ItemList.SelectedItems)
                {
                    PlaylistItem item = o as PlaylistItem;

                    if (item.Pos < targetPosition)
                    {
                        list.Commands.Add(new Command("moveid", item.Id, targetPosition - 1));
                        
                    }
                    else
                    {
                        list.Commands.Add(new Command("moveid", item.Id, targetPosition++));
                    }

                    World.Instance.CommandRunner.PushCommand(list);
                }
            }
        }

        #endregion
    }
}
