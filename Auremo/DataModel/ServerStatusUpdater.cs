using Auremo.Controls.Utility;
using Auremo.DataModel.AudioObjects;
using Auremo.Network;
using Auremo.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Auremo.DataModel
{
    public class ServerStatusUpdater : ServerConnectorBase
    {
        private volatile bool m_NeedToRefreshOutputs = true;
        private volatile bool m_NeedToRefreshStatus = true;
        private volatile bool m_NeedToRefreshStoredPlaylists = true;

        public ServerStatusUpdater() : base(false, "MPD server status poller")
        {
        }

        #warning TODO move this to a data store -- either ServerStatus or create a ConnectionStatus maybe.
        #warning TODO remove INotifyPropertyChanged
        public string MpdBanner
        {
            get => m_MpdBanner;
            set
            {
                if (m_MpdBanner != value)
                {
                    m_MpdBanner = value;
                    NotifyPropertyChanged("MpdBanner");
                }
            }
        }

        public bool IsConnected
        {
            get => m_IsConnected;
            private set
            {
                if (m_IsConnected != value)
                {
                    m_IsConnected = value;
                    NotifyPropertyChanged("IsConnected");
                }
            }
        }

        protected override void OnConnected()
        {
            m_Connection.PropertyChanged += OnConnectionPropertyChanged;
            IsConnected = true;
            MpdBanner = m_Connection.MpdBanner;

            m_NeedToRefreshOutputs = true;
            m_NeedToRefreshStatus = true;
            SendNextCommand();
        }

        protected override bool Work()
        {
            // Signal a ServerStatus update every now and then, because we won't be
            // updating it with real data very often.
            Thread.Sleep(100);
            ThreadUtility.RunInUiThread(() =>
            {
                World.Instance.ServerStatus.SignalUpdate();
            });

            // TODO: maybe put the "state machine" here, if at all possible.

            return true;
        }

        protected override void OnDisconnected()
        {
            m_Connection.PropertyChanged -= OnConnectionPropertyChanged;
            IsConnected = false;
            MpdBanner = "";

            ThreadUtility.RunInUiThread(() => World.Instance.ServerStatus.Update(new ServerStatus.StatusTemplate()));
        }

        private void OnConnectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                IsConnected = m_Connection != null && m_Connection.State == Connection.ConnectionState.Connected;
            }
            else if (e.PropertyName == "MpdBanner")
            {
                MpdBanner = m_Connection == null ? "" : m_Connection.MpdBanner;
            }
        }

        private void SendNextCommand()
        {
            // Figure out what to say to the MPD server next.

            if (m_NeedToRefreshOutputs)
            {
                // Outputs list is out of date.
                m_Connection.Send(new Command("outputs"), ReadOutputsResponse);
            }
            else if (m_NeedToRefreshStoredPlaylists)
            {
                // Status is out of date.
                m_Connection.Send(new Command("listplaylists"), ReadListPlaylistsResponse);
            }
            else if (m_NeedToRefreshStatus)
            {
                // Status is out of date.
                m_Connection.Send(new Command("status"), ReadStatusResponse);
            }
            else
            {
                // Nothing is out of date. Idle until the server reports something interesting.
                m_Connection.Send(new Command("idle",
                                              "database", "stored_playlist", "playlist", "player",
                                              "mixer", "output", "options"),
                                  ReadIdleResponse);
            }
        }

        private void ReadOutputsResponse(Sendable request, Response response)
        {
            if (response.IsOk && response.HasData)
            {
                // Parse the list in the worker thead.
                Output output = null;
                IList<Output> outputs = new List<Output>();

                // TODO: this assumes the data are in a particular order -- fix.
                foreach (Datum datum in response.Data)
                {
                    if (datum.Key == "outputid")
                    {
                        output = new Output() { Id = datum.IntValue() };
                    }
                    else if (datum.Key == "outputname")
                    {
                        output.Name = datum.Value;
                    }
                    else if (datum.Key == "outputenabled")
                    {
                        output.Enabled = datum.BoolValue();
                        outputs.Add(output);
                        output = null;
                    }
                }

                // If everything succeeded, publish it in the UI.
                ThreadUtility.RunInUiThread(() =>
                {
                    IList<Output> target = World.Instance.ServerStatus.Outputs;
                    target.Clear();

                    foreach (Output o in outputs)
                    {
                        target.Add(o);
                    }
                });

            }

            m_NeedToRefreshOutputs = false;
            SendNextCommand();
        }

        private void ReadListPlaylistsResponse(Sendable request, Response response)
        {
            if (response.IsOk)
            {
                IList<Playlist> playlists;

                if (response.HasData)
                {
                    playlists = AudioObjectParser.ParseObjectList<Playlist>(response.Data);
                }
                else
                {
                    playlists = new List<Playlist>();
                }
            
                // If everything succeeded, publish it in the UI.
                ThreadUtility.RunInUiThread(() =>
                {
                    IList<Playlist> target = World.Instance.PlaylistInfo.AllPlaylists;
                    target.Clear();

                    foreach (Playlist p in playlists)
                    {
                        target.Add(p);
                    }

                    World.Instance.PlaylistInfo.PlaylistEditTimestamp = DateTime.Now;
                });
            }

            m_NeedToRefreshStoredPlaylists = false;
            SendNextCommand();
        }

        private void ReadStatusResponse(Sendable request, Response response)
        {
            if (response.IsOk && response.HasData)
            {
                ServerStatus.StatusTemplate status = new ServerStatus.StatusTemplate();
                status.IsConnected = true;

                // Parse these in the worker thread first.
                status.Volume = response.ParseOptionalInt("volume").GetValueOrDefault(0);
                status.Repeat = response.ParseBool("repeat");
                status.Random = response.ParseBool("random");
                status.SingleMode = ParserUtility.StringToSingleMode(response.Parse("single"));
                status.Consume = response.ParseBool("consume");
                status.PlayQueueVersion = response.ParseInt("playlist");
                status.PlayQueueLength = response.ParseInt("playlistlength");
                status.PlayState = ParserUtility.StringToPlayState(response.Parse("state"));
                status.Song = response.ParseOptionalInt("song");
                status.SongId = response.ParseOptionalInt("songid");
                status.Elapsed = response.ParseOptionalDouble("elapsed");
                status.Duration = response.ParseOptionalDouble("duration");
                status.BitRate = response.ParseOptionalInt("bitrate");
                status.Xfade = response.ParseOptionalInt("xfade");
                status.MixRampThreshold = response.ParseDouble("mixrampdb");
                status.MixRampDelay = response.ParseOptionalDouble("mixrampdelay");
                status.AudioSampleRateBitsChannels = response.ParseOptional("audio");
                status.UpdatingDb = response.ParseOptionalInt("updating_db");
                status.Error = response.ParseOptional("error");

                // If everything succeeded, publish them in the UI.
                ThreadUtility.RunInUiThread(() =>
                {
                    World.Instance.ServerStatus.Update(status);
                });

                m_NeedToRefreshStatus = false;
            }
            else
            {
                World.Instance.Log.LogMessageFromThread("Failed to query MPD status.");
            }

            SendNextCommand();
        }

        private void ReadIdleResponse(Sendable request, Response response)
        {
            if (response.IsOk && response.HasData)
            {
                foreach (Datum datum in response.Data)
                {
                    if (datum.Key == "changed")
                    {
                        if (datum.Value == "output")
                        {
                            m_NeedToRefreshOutputs = true;
                        }
                        else if (datum.Value == "stored_playlist")
                        {
                            m_NeedToRefreshStoredPlaylists = true;
                        }
                        else
                        {
                            m_NeedToRefreshStatus = true;
                        }
                    }
                }
            }

            SendNextCommand();
        }

        private string m_MpdBanner = "";
        private bool m_IsConnected = false;
    }
}
