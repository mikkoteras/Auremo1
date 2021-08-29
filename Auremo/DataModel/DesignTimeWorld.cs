#if DEBUG

using Auremo.DataModel.AudioObjects;
using Auremo.DataModel.Types;
using Auremo.Network;
using System.Collections.Generic;

namespace Auremo.DataModel
{
    public class DesignTimeWorld
    {
        public class MockRemoteProperty<T> where T : struct
        {
            public MockRemoteProperty(T v) { ClientSideValue = v; ServerSideValue = v; }
            public T ClientSideValue { get; set; }
            public T ServerSideValue { get; set; }
        }

        public MockLog Log { get; } = new MockLog();

        public MockConfiguration Configuration { get; } = new MockConfiguration();

        public MockViewHistory ViewHistory { get; } = new MockViewHistory();

        public InterfaceState InterfaceState { get; } = new InterfaceState()
        {
            ViewMode = ViewMode.Activity
        };

        public MockServerStatus ServerStatus { get; } = new MockServerStatus();

        public MockActivity Activity { get; } = new MockActivity();

        public Translator Translator { get; } = new Translator("English");

        public MockQueryResult QueryResult { get; } = new MockQueryResult();

        public MockPlaylistInfo PlaylistInfo { get; } = new MockPlaylistInfo();

        public MockPlayQueue PlayQueue { get; } = new MockPlayQueue();
        
        #region Data

        public class MockLog
        {
            public IList<string> Entries => new List<string>()
            {
                "Everything is in order!",
                "No, wait, something went wrong.",
                "No wait, it's ok, carry on."
            };
        }

        public class MockConfiguration
        {
            public class MockActiveOptions
            {
                public VolumeControlType VolumeControlType => VolumeControlType.Slider;
                public bool VolumeControlTypeIsNone => VolumeControlType == VolumeControlType.None;
                public bool VolumeControlTypeIsWheel => VolumeControlType == VolumeControlType.Wheel;
                public bool VolumeControlTypeIsSlider => VolumeControlType == VolumeControlType.Slider;
                public bool VolumeControlTypeIsButtons => VolumeControlType == VolumeControlType.Buttons;
            }

            public MockActiveOptions ActiveOptions => new MockActiveOptions();
        }

        public class MockViewHistory
        {
            public ViewMode CurrentView => ViewMode.Activity;
            public Sendable CurrentSearch => null;
            public bool CanGoBack => true;
            public bool CanGoForward => false;
        }

        public class MockInterfaceState
        {
            public ViewMode ViewMode => ViewMode.PlayQueue;
            public bool ViewModeIsActivity => ViewMode == ViewMode.Activity;
            public bool ViewModeIsQueryResult => ViewMode == ViewMode.QueryResult;
            public bool ViewModeIsPlaylists => ViewMode == ViewMode.PlaylistContents;
            public bool ViewModeIsPlayQueue => ViewMode == ViewMode.PlayQueue;
            public bool ViewModeIsServer => ViewMode == ViewMode.Server;
            public bool ViewModeIsConfiguration => ViewMode == ViewMode.Configuration;
            public bool ViewModeIsDeveloper => ViewMode == ViewMode.Developer;
            public bool ViewModeIsSandbox => ViewMode == ViewMode.QueryResult;
            public bool PopupOverlayIsActive => false;
        }

        public class MockServerStatus
        {
            public bool IsConnected => true;
            public MockRemoteProperty<int> Volume { get; } = new MockRemoteProperty<int>(34);
            public bool Repeat => true;
            public bool Random => false;
            public SingleMode SingleMode => SingleMode.True;
            public bool Consume => true;
            public float? Elapsed => 122.3f;
            public float? Duration => 422.0f;
            public int? BitRate => 44110;
            public MockRemoteProperty<double> Xfade { get; } = new MockRemoteProperty<double>(5);
            public MockRemoteProperty<double> MixRampThreshold { get; } = new MockRemoteProperty<double>(0.0f);
            public MockRemoteProperty<double> MixRampDelay { get; } = new MockRemoteProperty<double>(5.511f);
            public string Error => "Error: no error!";
            public PlayState PlayState => PlayState.Playing;
            public List<Output> Outputs => new List<Output>()
            {
                new Output() { Id = 0, Name = "Man cave", Enabled = false },
                new Output() { Id = 1, Name = "Living room", Enabled = true },
                new Output() { Id = 2, Name = "Sauna", Enabled = true },
                new Output() { Id = 3, Name = "HDMI aux", Enabled = false },
                new Output() { Id = 4, Name = "Headphones (hidden behind the TV)", Enabled = false },
                new Output() { Id = 5, Name = "Filler", Enabled = false },
                new Output() { Id = 6, Name = "To", Enabled = false },
                new Output() { Id = 7, Name = "Bring", Enabled = false },
                new Output() { Id = 8, Name = "Up", Enabled = false },
                new Output() { Id = 9, Name = "Scrollbar", Enabled = false }
            };
        }

        public class MockActivity
        {
            public Track CurrentSong => new Track()
            {
                File = "music/enya/01-the_celts/12-boadicea.flac",
                Album = "The Celts",
                Artist = "Enya",
                Date = "1980",
                Title = "Boadicea"
            };
        }

        public class MockQueryResult
        {
            public Sendable Search => new Command("getsomerandomitems");
            public IList<AudioObject> SearchResult => new List<AudioObject>
            {
                new Artist("Wolfgang Amadeus Mozart"),
                new Artist("Goldfrapp"),
                new Artist("Death Cab for Cutie"),
                new Genre("Opera"),
                new Track()
                {
                    Artist = "Harvey Danger",
                    Album = "Where Have All The Merrymakers Gone?",
                    Title = "Jack The Lion",
                    Date = "1997"
                },new Track()
                {
                    Artist = "Wolfgang Amadeus Mozart",
                    Title = "Non più andrai",
                    Date = "1786"
                },
                new Track()
                {
                    Artist = "Harvey Danger",
                    Album = "Where Have All The Merrymakers Gone?",
                    Title = "Wrecking Ball",
                    Date = "1997"
                },
                new Directory("vangelis/02-el_greco"),
                new Directory("vangelis/03-voices"),
                new Playlist("radio-stations.M3U")
            };
        }

        public class MockPlaylistInfo
        {
            private readonly Playlist someStuff = new Playlist("some-stuff.m3u");
            public IList<Playlist> AllPlaylists => new List<Playlist>()
            {
                new Playlist("radio-stations.m3u"),
                someStuff,
                new Playlist("running-tracks.m3u")
            };
            public Playlist SelectedPlaylist => someStuff;
            public IList<PlaylistItem> SelectedPlaylistContents => new List<PlaylistItem>()
            {
                new PlaylistItem()
                {
                    Album = "Lovers Rock",
                    Artist = "Sade",
                    Title = "By Your Side",
                    Pos = 0
                },
                new PlaylistItem()
                {
                    Album = "Lovers Rock",
                    Artist = "Sade",
                    Title = "Flow"
                },
                new PlaylistItem()
                {
                    Album = "Lovers Rock",
                    Artist = "Sade",
                    Title = "King of Sorrow",
                    IsAboveDropTarget = true
                },
                new PlaylistItem()
                {
                    Album = "Lovers Rock",
                    Artist = "Sade",
                    Title = "Somebody Already Broke My Heart",
                    IsBelowDropTarget = true
                },
                new PlaylistItem()
                {
                    Album = "Lovers Rock",
                    Artist = "Sade",
                    Title = "All About Our Love"
                },
                new PlaylistItem()
                {
                    Album = "Lovers Rock",
                    Artist = "Sade",
                    Title = "Slave Song"
                }
            };
        }

        public class MockPlayQueue
        {
            public IList<PlaylistItem> Items => new List<PlaylistItem>()
            {
                new PlaylistItem()
                {
                    Album = "Ágætis byrjun",
                    Artist = "Sigur Rós",
                    Title = "Viðrar vel til loftárása",
                    Pos = 0
                },
                new PlaylistItem()
                {
                    Album = "Pop",
                    Artist = "U2",
                    Title = "Gone",
                    IsPaused = true
                },
                new PlaylistItem()
                {
                    Album = "Little By Little...",
                    Artist = "Harvey Danger",
                    Title = "Cream and Bastards Rise"
                },
                new PlaylistItem()
                {
                    Album = "Lifeforms (EP)",
                    Artist = "FSOL",
                    Title = "Lifeforms (Path 1)",
                    IsPlaying = true
                },
                new PlaylistItem()
                {
                    Album = "Lifeforms (EP)",
                    Artist = "FSOL",
                    Title = "Lifeforms (Path 2)"
                },
                new PlaylistItem()
                {
                    Album = "Lifeforms (EP)",
                    Artist = "FSOL",
                    Title = "Lifeforms (Path 3)"
                },
                new PlaylistItem()
                {
                    Album = "Lifeforms (EP)",
                    Artist = "FSOL",
                    Title = "Lifeforms (Path 4)"
                },
                new PlaylistItem()
                {
                    Album = "Lifeforms (EP)",
                    Artist = "FSOL",
                    Title = "Lifeforms (Path 5)",
                    IsAboveDropTarget = true
                },
                new PlaylistItem()
                {
                    Album = "Lifeforms (EP)",
                    Artist = "FSOL",
                    Title = "Lifeforms (Path 6)",
                    IsBelowDropTarget = true
                },
                new PlaylistItem()
                {
                    Album = "Lifeforms (EP)",
                    Artist = "FSOL",
                    Title = "Lifeforms (Path 7)"
                }
            };
        }

        #endregion
    }
}

#endif
