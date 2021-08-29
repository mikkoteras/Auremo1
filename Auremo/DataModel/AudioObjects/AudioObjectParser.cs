using Auremo.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auremo.DataModel.AudioObjects
{
    // Various helper methods to convert a list of data lines to an
    // IAudioObject or a list of IAudioObjects. Each method removes
    // the "used" lines from the argument data.
    // TODO: work in progress.
    public static class AudioObjectParser
    {
        private static List<string> m_NewObjectHeaders = new List<string>()
        {
            "file",
            "playlist",
            "directory"
        };

        public static IList<T> ParseObjectList<T>(IList<Datum> data) where T : AudioObject
        {
            IList<T> result = new List<T>();

            while (data.Count > 0)
            {
                Datum header = data.First();

                if (header.Key == "file")
                {
                    result.Add(CastObject<Track, T>(ParseTrack(data)));
                }
                else if (header.Key == "playlist")
                {
                    result.Add(CastObject<Playlist, T>(ParsePlaylistObject(data)));
                }
            }

            return result;
        }

        public static IList<PlaylistItem> ParsePlaylist(IList<Datum> data)
        {
            IList<PlaylistItem> result = new List<PlaylistItem>();

            while (data.Count > 0)
            {
                Datum header = data.First();

                if (header.Key == "file")
                {
                    PlaylistItem item = ParsePlaylistItem(data);
                    item.Pos = result.Count;
                    result.Add(item);
                }
                else
                {
                    throw new Exception($"AudioObjectParser.ParsePlaylist(): unexpected header \"{header}\".");
                }
            }

            return result;
        }

        private static U CastObject<T, U>(T source) where T : AudioObject
                                                    where U : AudioObject
        {
            if (source is U target)
            {
                return target;
            }
            else
            {
                throw new Exception("AudioObjectParser.Cast(): encountered unexpected object type.");
            }
        }

        public static Track ParseTrack(IList<Datum> data)
        {
            Track result = null;

            if (data.Count > 0)
            {
                Datum datum = data.First();
                data.RemoveAt(0);

                if (datum.Key != "file")
                {
                    throw new Exception($"ParseTrack(): unexpected leading key: {datum.Key}");
                }

                result = new Track()
                {
                    File = datum.Value
                };

                while (data.Count > 0 && !m_NewObjectHeaders.Contains(data.First().Key))
                {
                    datum = data.First();
                    data.RemoveAt(0);

                    if (datum.Key == "Artist")
                    {
                        result.Artist = datum.Value;
                    }
                    else if (datum.Key == "Album")
                    {
                        result.Album = datum.Value;
                    }
                    else if (datum.Key == "Date")
                    {
                        result.Date = datum.Value;
                    }
                    else if (datum.Key == "Title")
                    {
                        result.Title = datum.Value;
                    }
                }
            }

            return result;
        }

        // Sorry about the derpy name, but we also need a ParsePlaylist
        // that returns a list of objects.
        public static Playlist ParsePlaylistObject(IList<Datum> data)
        {
            Playlist result = null;

            if (data.Count > 0)
            {
                Datum datum = data.First();
                data.RemoveAt(0);

                if (datum.Key != "playlist")
                {
                    throw new Exception($"ParsePlaylist(): unexpected leading key: {datum.Key}");
                }

                result = new Playlist(datum.Value);

                while (data.Count > 0 && !m_NewObjectHeaders.Contains(data.First().Key))
                {
                    data.RemoveAt(0);
                }
            }

            return result;
        }

        public static PlaylistItem ParsePlaylistItem(IList<Datum> data)
        {
            PlaylistItem result = null;

            if (data.Count > 0)
            {
                Datum datum = data.First();
                data.RemoveAt(0);

                if (datum.Key != "file")
                {
                    throw new Exception($"ParsePlaylistItem(): unexpected leading key: {datum.Key}");
                }

                result = new PlaylistItem()
                {
                    File = datum.Value
                };

                while (data.Count > 0 && !m_NewObjectHeaders.Contains(data.First().Key))
                {
                    datum = data.First();
                    data.RemoveAt(0);

                    if (datum.Key == "Artist")
                    {
                        result.Artist = datum.Value;
                    }
                    else if (datum.Key == "Album")
                    {
                        result.Album = datum.Value;
                    }
                    else if (datum.Key == "Title")
                    {
                        result.Title = datum.Value;
                    }
                    else if (datum.Key == "Pos")
                    {
                        result.Pos = datum.IntValue();
                    }
                    else if (datum.Key == "Id")
                    {
                        result.Id = datum.IntValue();
                    }
                }
            }

            return result;
        }
    }
}
