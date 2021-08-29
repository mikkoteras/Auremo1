using Auremo.Controls.Utility;
using Auremo.DataModel.AudioObjects;
using Auremo.DataModel.Types;
using Auremo.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Auremo.DataModel.DataStores
{
    public class PlaylistInfo : RestorableDataStore, INotifyPropertyChanged
    {
        public class PlaylistViewDefinition : ViewDefinition
        {
            public PlaylistViewDefinition(ViewMode view, RestorableDataStore creator, Playlist playlist) : base(view, creator)
            {
                Playlist = playlist;
            }

            public Playlist Playlist
            {
                get;
                set;
            }
        }

        public PlaylistInfo()
        {
            AllPlaylists = new ObservableCollection<Playlist>();
            SelectedPlaylistContents = new ObservableCollection<PlaylistItem>();
            PropertyChanged += OnPropertyChanged;
        }

        public ObservableCollection<Playlist> AllPlaylists
        {
            get;
            private set;
        }

        public Playlist SelectedPlaylist
        {
            get => m_SelectedPlaylist;
            set
            {
                if (m_SelectedPlaylist != value)
                {
                    m_SelectedPlaylist = value;
                    NotifyPropertyChanged("SelectedPlaylist");
                }
            }
        }

        public ObservableCollection<PlaylistItem> SelectedPlaylistContents
        {
            get;
            private set;
        }

        public DateTime PlaylistEditTimestamp
        {
            get => m_PlaylistEditTimestamp;
            set
            {
                if (m_PlaylistEditTimestamp != value)
                {
                    m_PlaylistEditTimestamp = value;
                    NotifyPropertyChanged("PlaylistEditTimestamp");
                }
            }
        }

        public void DisplayView(Playlist playlist)
        {
            SelectedPlaylist = SecurePlaylist(playlist);
            World.Instance.InterfaceState.ViewMode = ViewMode.PlaylistContents;
            World.Instance.ViewHistory.PushView(new PlaylistViewDefinition(ViewMode.PlaylistContents, this, playlist));
        }

        public override void RestoreFromDefinition(ViewDefinition definition)
        {
            if (definition is PlaylistViewDefinition def && def.Creator == this)
            {
                SelectedPlaylist = SecurePlaylist(def.Playlist);
                World.Instance.InterfaceState.ViewMode = ViewMode.PlaylistContents;
            }
            else
            {
                throw new Exception("PlaylistInfo.RestoreFromDefinition(): Logic error!");
            }
        }

        /// Find the available playlist with the same file as the argument's, or null.
        /// This is necessary because the selected playlist must be reference-equal to
        /// one of the playlists in the catalog, and the catalogue can be resent at any
        /// time by the server.
        public Playlist SecurePlaylist(Playlist playlist)
        {
            return AllPlaylists.FirstOrDefault(p => p.File == playlist.File);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedPlaylist" || e.PropertyName == "PlaylistEditTimestamp")
            {
                SelectedPlaylistContents.Clear();

                if (SelectedPlaylist != null)
                {
                    #warning TODO use something better than QueryRunner.
                    World.Instance.QueryRunner.PushQuery(new Command("listplaylistinfo", SelectedPlaylist.File), OnPlaylistInfoReceived);
                }
            }
        }

        private void OnPlaylistInfoReceived(Sendable request, Response response)
        {
            if (response.IsOk && response.HasData)
            {
                IList<PlaylistItem> items = AudioObjectParser.ParsePlaylist(response.Data);
                ThreadUtility.RunInUiThread(() =>
                {
                    foreach (PlaylistItem item in items)
                    {
                        SelectedPlaylistContents.Add(item);
                    }
                });
            }
            else
            {
                #warning TODO log failures somewhere.
            }
        }

        private Playlist m_SelectedPlaylist = null;
        private DateTime m_PlaylistEditTimestamp = DateTime.MinValue;

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
