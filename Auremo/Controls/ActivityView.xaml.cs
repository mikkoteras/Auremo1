using Auremo.DataModel;
using Auremo.DataModel.AudioObjects;
using Auremo.Network;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Auremo.Controls
{
    public partial class ActivityView : UserControl
    {
        private byte[] AlbumArtBytes = null;
        private string m_AlbumArtFilename = null;
        private int m_AlbumArtBytesReceived = 0;

        public ActivityView()
        {
            AlbumArtBytes = null;
            InitializeComponent();

            // This is null in Designer, but never at runtime.
            if (World.Instance != null)
            {
                World.Instance.ServerStatus.PropertyChanged += ServerStatusChanged;
                // TODO: connection up/down
                UpdateCurrentSong();
            }
        }

        private void ServerStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Song")
            {
                AlbumArtBytes = null;
                World.Instance.Activity.AlbumArtSize = -1;
                UpdateCurrentSong();
            }
        }

        private void UpdateCurrentSong()
        {
            World.Instance.CommandRunner.PushQuery(new Command("currentsong"), OnCurrentSongResponseReceived);
        }

        private void OnCurrentSongResponseReceived(Sendable command, Response response)
        {
            if (response.IsOk)
            {
                World.Instance.Activity.CurrentSong = AudioObjectParser.ParseTrack(response.Data);

                if (World.Instance.Activity.CurrentSong != null)
                {
                    m_AlbumArtFilename = World.Instance.Activity.CurrentSong.File;
                    m_AlbumArtBytesReceived = 0;
                    World.Instance.CommandRunner.PushQuery(new Command("albumart", m_AlbumArtFilename, "0"), OnAlbumArtResponseReceived);
                }
            }
            else
            {
                World.Instance.Activity.CurrentSong = new Track();
                AlbumArtBytes = null;
                World.Instance.Activity.AlbumArtSize = -1;
            }
        }

        private void OnAlbumArtResponseReceived(Sendable request, Response response)
        {
            #warning TODO: this method is desperately in need of a cleanup!

            Command command = request as Command;

            // Command is still relevant?
            if (command.Arguments[0].ToString() == m_AlbumArtFilename)
            {
                if (response.IsOk)
                {
                    int startByteIndex = (int)command.Arguments[1];

                    if (startByteIndex == 0)
                    {
                        // This is the first binary chunk for this image.
                        Datum sizeDatum = response.FindFirstDatumWithKey("size");

                        if (sizeDatum == null)
                        {
                            // This should not happen. Bail out.
                            World.Instance.Log.LogMessage("OnAlbumArtResponseReceived(): Image size not received");
                            AlbumArtBytes = null;
                            World.Instance.Activity.AlbumArtSize = -1;
                        }

                        int totalSize = sizeDatum.IntValue();
                        AlbumArtBytes = new byte[totalSize];
                        response.BinaryObject.CopyTo(AlbumArtBytes, 0);
                        m_AlbumArtBytesReceived += response.BinaryObject.Length;

                        if (m_AlbumArtBytesReceived < totalSize)
                        {
                            World.Instance.CommandRunner.PushQuery(new Command("albumart", m_AlbumArtFilename, m_AlbumArtBytesReceived.ToString()),
                                                                   OnAlbumArtResponseReceived);
                        }
                    }
                    else
                    {
                        // This is a followup binary chunk for this image.
                        response.BinaryObject.CopyTo(AlbumArtBytes, m_AlbumArtBytesReceived);
                        m_AlbumArtBytesReceived += response.BinaryObject.Length;

                        if (m_AlbumArtBytesReceived < AlbumArtBytes.Length)
                        {
                            World.Instance.CommandRunner.PushQuery(new Command("albumart", m_AlbumArtFilename, m_AlbumArtBytesReceived.ToString()),
                                                                   OnAlbumArtResponseReceived);
                        }

                        World.Instance.Activity.AlbumArtSize = m_AlbumArtBytesReceived;
                    }

                    if (m_AlbumArtBytesReceived == AlbumArtBytes.Length && AlbumArtBytes.Length > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = new MemoryStream(AlbumArtBytes);
                            bitmap.EndInit();
                            World.Instance.Activity.AlbumCover = bitmap;
                        });
                        
                        World.Instance.Activity.AlbumArtSize = m_AlbumArtBytesReceived;
                    }
                }
                else
                {
                    // Probably means there is no album art. No worries.
                    AlbumArtBytes = null;
                    World.Instance.Activity.AlbumCover = null;
                    World.Instance.Activity.AlbumArtSize = -1;
                }
            }
        }
    }
}
