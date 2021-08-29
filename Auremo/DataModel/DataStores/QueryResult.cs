using Auremo.DataModel.AudioObjects;
using Auremo.DataModel.Types;
using Auremo.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Auremo.DataModel.DataStores
{
    public class QueryResult : RestorableDataStore, INotifyPropertyChanged
    {
        public class QueryResultViewDefinition : ViewDefinition
        {
            public QueryResultViewDefinition(ViewMode view, RestorableDataStore creator, Sendable search) : base(view, creator)
            {
                Search = search;
            }

            public Sendable Search
            {
                get;
                private set;
            }
        }

        public QueryResult()
        {
            SearchResult = new ObservableCollection<AudioObject>();
            PropertyChanged += OnPropertyChanged;
        }

        public Sendable Search
        {
            get => m_Search;
            set
            {
                if (m_Search != value)
                {
                    m_Search = value;
                    NotifyPropertyChanged("Search");
                }
            }
        }

        public ObservableCollection<AudioObject> SearchResult
        {
            get;
            private set;
        }

        public void DisplayView(Sendable search)
        {
            Search = search;
            World.Instance.InterfaceState.ViewMode = ViewMode.QueryResult;
            World.Instance.ViewHistory.PushView(new QueryResultViewDefinition(ViewMode.QueryResult, this, search));
        }

        public override void RestoreFromDefinition(ViewDefinition definition)
        {
            if (definition is QueryResultViewDefinition def && def.Creator == this)
            {
                Search = def.Search;
                World.Instance.InterfaceState.ViewMode = ViewMode.QueryResult;
            }
            else
            {
                throw new Exception("QueryResult.RestoreFromDefinition(): Logic error!");
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Search")
            {
                SearchResult.Clear();

                if (Search != null)
                {
                    #warning TODO: there really should be a more sophisticated mechanism for general-use connections.
                    World.Instance.QueryRunner.PushQuery(Search, OnSearchResultReceived);
                }
            }
        }

        private void OnSearchResultReceived(Sendable request, Response response)
        {
            #warning TODO use something better than QueryRunner.
            Application.Current.Dispatcher.BeginInvoke((Connection.ResponseCallback)ProcessQueryResponse, request, response);
        }

        private Sendable m_Search = null;

        #region Parser

        private static readonly List<string> CommandsWithLineResponses = new List<string>()
        {
            "list"
        };

        private static readonly List<string> CommandsWithBlockResponses = new List<string>()
        {
            "lsinfo", "find", "search", "listplaylists", "listplaylistinfo"
        };

        private void ProcessQueryResponse(Sendable request, Response response)
        {
            Command command = request as Command;

            if (response.IsOk)
            {
                if (CommandsWithLineResponses.Contains(command.Verb))
                {
                    ProcessLineFormResponse(response);
                }
                else if (CommandsWithBlockResponses.Contains(command.Verb))
                {
                    ProcessBlockFormResponse(response);
                }
                else
                {
                    throw new Exception("QueryRunner.ProcessQueryResponse: unexpected command: " + command);
                }
            }
            else
            {
                StringBuilder message = new StringBuilder();
                message.Append("MPD query failed. Command: \"");
                message.Append(command);
                message.Append("\" Response: \"");
                message.Append(response.AckMessage);
                message.Append("\"");
                World.Instance.Log.LogMessageFromThread(message.ToString());
            }
        }

        private void ProcessLineFormResponse(Response response)
        {
            // In this type of response, one object is described with a single line.
            // For example:
            //     file: path/to/file1.flac
            //     Artist: Artist2
            //     Genre: Genre3
            //     OK
            if (response.HasData)
            {
                IList<Datum> dataAsList = new List<Datum>();

                foreach (Datum datum in response.Data)
                {
                    dataAsList.Add(datum);
                    SearchResult.Add(DataBlockToAudioObject(dataAsList));
                    dataAsList.Clear();
                }
            }
        }

        private void ProcessBlockFormResponse(Response response)
        {
            // In this type of response, one object is described with multiple lines.
            // Some special keys denote the beginning of a new object. For example:
            //     file: path/to/object1
            //     PropertyA: property A of object 1 (a file)
            //     PropertyB: property 2 of object 1 (a file)
            //     directory: path/to/object2
            //     PropertyA: property A of object 1 (a directory)
            //     OK
            if (response.HasData)
            {
                IList<Datum> data = new List<Datum>();

                foreach (Datum datum in response.Data)
                {
                    if (data.Count > 0 && (datum.Key == "file" || datum.Key == "directory" || datum.Key == "playlist"))
                    {
                        SearchResult.Add(DataBlockToAudioObject(data));
                        data.Clear();
                    }

                    data.Add(datum);
                }

                if (data.Count > 0)
                {
                    SearchResult.Add(DataBlockToAudioObject(data));
                }
            }
        }

        // Convert a list of data describing a single object to a
        // real object. Its type is identified by the first datum.
        private AudioObject DataBlockToAudioObject(IList<Datum> data)
        {
            // TODO: this stuff *seriously* need to go to the IAudioObject
            // subclasses themselves. This is bad design.
            Datum typeDatum = data[0];

            if (typeDatum.Key == "Artist")
            {
                return new Artist(typeDatum.Value);
            }
            else if (typeDatum.Key == "Genre")
            {
                return new Genre(typeDatum.Value);
            }
            else if (typeDatum.Key == "directory")
            {
                return new Directory(typeDatum.Value);
            }
            else if (typeDatum.Key == "playlist")
            {
                return new Playlist(typeDatum.Value);
            }
            else if (typeDatum.Key == "file")
            {
                Track track = new Track();

                // TODO: yuck!
                foreach (Datum attribute in data)
                {
                    if (attribute.Key == "Title")
                    {
                        track.Title = attribute.Value;
                    }
                    else if (attribute.Key == "Artist")
                    {
                        track.Artist = attribute.Value;
                    }
                    else if (attribute.Key == "Album")
                    {
                        track.Album = attribute.Value;
                    }
                    else if (attribute.Key == "Date")
                    {
                        track.Date = attribute.Value;
                    }
                    else if (attribute.Key == "file")
                    {
                        track.File = attribute.Value;
                    }
                };

                return track;
            }

            // TODO some other exception type
            throw new Exception("DataBlockToAudioObject() failed.");
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }



    

}
