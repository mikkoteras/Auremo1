using Auremo.Controls.Utility;
using Auremo.DataModel.AudioObjects;
using Auremo.DataModel.Types;
using Auremo.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Auremo.DataModel
{
    #warning TODO rip the connection out of here.

    public class PlayQueue : ServerConnectorBase
    {
        private int? m_ServerStatusSong = null;

        public PlayQueue() : base(false, "Play queue poller")
        {
            Items = new ObservableCollection<PlaylistItem>();
            World.Instance.ServerStatus.PropertyChanged += OnServerStatusChanged;
        }

        public ObservableCollection<PlaylistItem> Items
        {
            get;
            private set;
        }

        protected override void OnConnected()
        {
            QueryPlayQueue();
        }

        protected override void OnDisconnected()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)ClearPlayQueue);
        }

        private void ClearPlayQueue()
        {
            Items.Clear();
        }

        private void OnServerStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PlayQueueVersion")
            {
                QueryPlayQueue();
            }
            else if (e.PropertyName == "Song")
            {
                ServerStatusSong = World.Instance.ServerStatus.Song;
            }
            else if (e.PropertyName == "PlayState")
            {
                SetItemPlayState(ServerStatusSong);
            }
        }

        private void QueryPlayQueue()
        {
            m_Connection?.Send(new Command("playlistinfo"), OnResultReceived);
        }

        public void OnResultReceived(Sendable request, Response response)
        {
            // Note that we may theoretically have disconnected while waiting
            // for the response.
            if (m_Connection != null)
            {
                #warning TODO: maybe parse the response in the component thread context before sending it to the GUI thread, to get a bit of extra concurrency benefit.
                Application.Current.Dispatcher.BeginInvoke((Connection.ResponseCallback)ReadPlaylistInfoResponse, request, response);
            }
        }

        public void ReadPlaylistInfoResponse(Sendable request, Response response)
        {
            Items.Clear();

            if (response.IsOk)
            {
                if (response.HasData)
                {
                    IList<PlaylistItem> playlist = AudioObjectParser.ParsePlaylist(response.Data);

                    ThreadUtility.RunInUiThread(() =>
                    {
                        foreach (PlaylistItem item in playlist)
                        {
                            Items.Add(item);
                        }
                    });
                }
            }

            SetItemPlayState(ServerStatusSong);
        }

        private int? ServerStatusSong
        {
            get => m_ServerStatusSong;
            set
            {
                SetItemPlayState(m_ServerStatusSong);
                
                if (m_ServerStatusSong != value)
                {
                    m_ServerStatusSong = value;
                    SetItemPlayState(m_ServerStatusSong);
                }
            }
        }

        private void SetItemPlayState(int? itemIndex)
        {
            if (itemIndex.HasValue && itemIndex.Value < Items.Count)
            {
                PlaylistItem item = Items[itemIndex.Value];
                int activeItemPos = World.Instance.ServerStatus.Song.GetValueOrDefault(-1);

                if (item.Pos == activeItemPos)
                {
                    PlayState playState = World.Instance.ServerStatus.PlayState;
                    item.IsPlaying = playState == PlayState.Playing;
                    item.IsPaused = playState == PlayState.Paused;
                }
                else
                {
                    item.IsPlaying = false;
                    item.IsPaused = false;
                }
            }
        }
    }
}
