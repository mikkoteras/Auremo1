using Auremo.Controls.Utility;
using Auremo.DataModel.Types;
using Auremo.Network;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Auremo.DataModel
{
    // All the status parameters in a response to an MPD "status" command.
    public class ServerStatus : INotifyPropertyChanged
    {
        public class StatusTemplate
        {
            public bool IsConnected { get; set; } = false;
            public int Volume { get; set; } = 0;
            public bool Repeat { get; set; } = false;
            public bool Random { get; set; } = false;
            public SingleMode SingleMode { get; set; } = SingleMode.False;
            public bool Consume { get; set; } = false;
            public int PlayQueueVersion { get; set; } = -1;
            public int PlayQueueLength { get; set; } = 0;
            public PlayState PlayState { get; set; } = PlayState.Unknown;
            public int? Song { get; set; } = null;
            public int? SongId { get; set; } = null;
            // TODO make this a class with a timestamp
            public double? Elapsed { get; set; } = null;
            public double? Duration { get; set; } = null;
            public int? BitRate { get; set; } = null;
            public int? Xfade { get; set; } = null;
            public double MixRampThreshold  { get; set; } = 0.0;
            public double? MixRampDelay { get; set; } = 0.0;
            public string AudioSampleRateBitsChannels { get; set; } = null;
            public int? UpdatingDb { get; set; } = null;
            public string Error { get; set; } = null;
        }

        public ServerStatus()
        {
            m_ElapsedStopwatch.Start();
            Outputs = new ObservableCollection<Output>();
        }

        public void Update(StatusTemplate status)
        {
            IsConnected = status.IsConnected;
            Volume.ServerSideValue = status.Volume;
            Repeat = Repeat;
            Random = status.Random;
            SingleMode = status.SingleMode;
            Consume = status.Consume;
            PlayQueueVersion = status.PlayQueueVersion;
            PlayQueueLength = status.PlayQueueLength;
            PlayState = status.PlayState;
            Song = status.Song;
            SongId = status.SongId;
            Elapsed = status.Elapsed;
            Duration = status.Duration;
            BitRate = status.BitRate;
            Xfade.ServerSideValue = status.Xfade;
            MixRampThreshold.ServerSideValue = status.MixRampThreshold;
            MixRampDelay.ServerSideValue = status.MixRampDelay;
            AudioSampleRateBitsChannels = status.AudioSampleRateBitsChannels;
            UpdatingDb = status.UpdatingDb;
            Error = status.Error;
        }

        public bool IsConnected
        {
            get => m_IsConnected;
            set
            {
                if (m_IsConnected != value)
                {
                    m_IsConnected = value;
                    NotifyPropertyChanged("IsConnected");
                }
            }
        }

        // 0..100
        public RemoteProperty<int> Volume { get; } =
            new RemoteProperty<int>(0, (int vol) => new Command("setvol", vol));

        public bool Repeat
        {
            get => m_Repeat;
            set
            {
                if (m_Repeat != value)
                {
                    m_Repeat = value;
                    NotifyPropertyChanged("Repeat");
                }
            }
        }

        public bool Random
        {
            get => m_Random;
            set
            {
                if (m_Random != value)
                {
                    m_Random = value;
                    NotifyPropertyChanged("Random");
                }
            }
        }

        public SingleMode SingleMode
        {
            get => m_SingleMode;
            set
            {
                if (m_SingleMode != value)
                {
                    m_SingleMode = value;
                    NotifyPropertyChanged("SingleMode");
                }
            }
        }

        public bool Consume
        {
            get => m_Consume;
            set
            {
                if (m_Consume != value)
                {
                    m_Consume = value;
                    NotifyPropertyChanged("Consume");
                }
            }
        }


        public int PlayQueueVersion
        {
            get => m_PlayQueueVersion;
            set
            {
                if (m_PlayQueueVersion != value)
                {
                    m_PlayQueueVersion = value;
                    NotifyPropertyChanged("PlayQueueVersion");
                }
            }
        }

        public int PlayQueueLength
        {
            get => m_PlayQueueLength;
            set
            {
                if (m_PlayQueueLength != value)
                {
                    m_PlayQueueLength = value;
                    NotifyPropertyChanged("PlayQueueLength");
                }
            }
        }

        public PlayState PlayState
        {
            get => m_PlayState;
            set
            {
                if (m_PlayState != value)
                {
                    m_PlayState = value;
                    NotifyPropertyChanged("PlayState");
                    NotifyPropertyChanged("IsPlaying"); // TODO remove?
                    NotifyPropertyChanged("IsPaused"); // TODO remove?
                    NotifyPropertyChanged("IsStopped"); // TODO remove?
                }
            }
        }

        /*
        public bool IsPlaying => PlayState == PlayState.Playing; // TODO remove?
        public bool IsPaused => PlayState == PlayState.Paused; // TODO remove?
        public bool IsStopped => PlayState == PlayState.Stopped; // TODO remove?
        */

        public int? Song
        {
            get => m_Song;
            set
            {
                if (m_Song != value)
                {
                    m_Song = value;
                    NotifyPropertyChanged("Song");
                }
            }
        }

        public int? SongId
        {
            get => m_SongId;
            set
            {
                if (m_SongId != value)
                {
                    m_SongId = value;
                    NotifyPropertyChanged("SongId");
                }
            }
        }

        public double? Elapsed
        {
            get
            {
                // Instead of polling the server status, we only get updates when something
                // significant happens, and the elapsed timecode isn't significant. We therefore
                // save a timestamp of the last seen timecode and use it to compute the actual
                // timecode, until the audio is stopped or paused, the track changes, etc.
                if (PlayState == PlayState.Playing)
                {
                    if (Duration == null || m_Elapsed == null)
                    {
                        return null;
                    }

                    return m_Elapsed + Math.Min(Duration.Value, (double)m_ElapsedStopwatch.Elapsed.TotalSeconds);
                }
                else
                {
                    return m_Elapsed;
                }
            }
            set
            {
                if (m_Elapsed != value)
                {
                    m_ElapsedStopwatch.Restart();
                    m_Elapsed = value;
                    NotifyPropertyChanged("Elapsed");
                }
            }
        }

        public double? Duration
        {
            get => m_Duration;
            set
            {
                if (m_Duration != value)
                {
                    m_Duration = value;
                    NotifyPropertyChanged("Duration");
                }
            }
        }

        public int? BitRate
        {
            get => m_BitRate;
            set
            {
                if (m_BitRate != value)
                {
                    m_BitRate = value;
                    NotifyPropertyChanged("BitRate");
                }
            }
        }

        public RemoteProperty<int> Xfade { get; } =
            new RemoteProperty<int>(0, (int val) => new Command("crossfade", val));

        public RemoteProperty<double> MixRampThreshold { get; } =
            new RemoteProperty<double>(0.0f, (double val) => new Command("mixrampdb", val));

        public RemoteProperty<double> MixRampDelay { get; } =
            new RemoteProperty<double>(0.0f, (double val) => new Command("mixrampdelay", val));

        public string AudioSampleRateBitsChannels
        {
            get => m_AudioSampleRateBitsChannels;
            set
            {
                if (m_AudioSampleRateBitsChannels != value)
                {
                    m_AudioSampleRateBitsChannels = value;
                    NotifyPropertyChanged("AudioSampleRateBitsChannels");
                }
            }
        }

        public int? UpdatingDb
        {
            get => m_UpdatingDb;
            set
            {
                if (m_UpdatingDb != value)
                {
                    m_UpdatingDb = value;
                    NotifyPropertyChanged("UpdatingDb");
                }
            }
        }

        public string Error
        {
            get => m_Error;
            set
            {
                if (m_Error != value)
                {
                    m_Error = value;
                    NotifyPropertyChanged("Error");
                }
            }
        }

        public ObservableCollection<Output> Outputs
        {
            get;
            private set;
        }

        // Signal event subscribers to refresh the properties that
        // haven't necessarily been really updated, but will be
        // computed from saved timestamps.
        public void SignalUpdate()
        {
            if (PlayState == PlayState.Playing)
            {
                NotifyPropertyChanged("Elapsed");
            }
        }

        private bool m_IsConnected = false;

        private bool m_Repeat = false;
        private bool m_Random = false;
        private SingleMode m_SingleMode = Types.SingleMode.Undefined;
        private bool m_Consume = false;
        private int m_PlayQueueVersion = -1;
        private int m_PlayQueueLength = 0;
        private PlayState m_PlayState = PlayState.Stopped;
        private int? m_Song = null;
        private int? m_SongId = null;
        private double? m_Elapsed = null;
        private double? m_Duration = null;
        private int? m_BitRate = null;
        private string m_AudioSampleRateBitsChannels = null;
        private int? m_UpdatingDb = null;
        private string m_Error = null;

        private readonly Stopwatch m_ElapsedStopwatch = new Stopwatch();

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
