using Auremo.DataModel.AudioObjects;
using Auremo.DataModel.Types;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Auremo.DataModel.DataStores
{
    public class Activity : INotifyPropertyChanged
    {
        private Track m_CurrentSong = new Track();
        private ImageSource m_AlbumCover = null;
        private int m_AlbumArtSize = -1;

        public Activity()
        {
        }

        public Track CurrentSong
        {
            get => m_CurrentSong;
            set
            {
                if (m_CurrentSong != value)
                {
                    m_CurrentSong = value;
                    NotifyPropertyChanged("CurrentSong");
                }
            }
        }

        public ImageSource AlbumCover
        {
            get => m_AlbumCover;
            set
            {
                if (m_AlbumCover != value)
                {
                    m_AlbumCover = value;
                    NotifyPropertyChanged("AlbumCover");
                }
            }
        }

        public int AlbumArtSize
        {
            get => m_AlbumArtSize;
            set
            {
                if (m_AlbumArtSize != value)
                {
                    m_AlbumArtSize = value;
                    NotifyPropertyChanged("AlbumArtSize");
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
