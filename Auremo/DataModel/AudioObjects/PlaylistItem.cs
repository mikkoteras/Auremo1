using Auremo.Network;
using System.ComponentModel;
using System.IO;

namespace Auremo.DataModel.AudioObjects
{
    public class PlaylistItem : AudioObject, INotifyPropertyChanged
    {
        public PlaylistItem()
        {
        }

        // In case this is a stream or otherwise doesn't have a title.
        public string TitleOrFile => Title ?? File;

        public string File
        {
            get;
            set;
        } = null;

        public string Title
        {
            get;
            set;
        } = null;

        public string Artist
        {
            get;
            set;
        } = null;

        public string Album
        {
            get;
            set;
        } = null;

        // Pos is given only for an item in the play queue, but required
        // by many commands related to playlists. Fortunately they are
        // trivial to generate on the client side.
        public int Pos
        {
            get;
            set;
        } = -1;

        // IDs are only applicable to items in the play queue.
        public int Id
        {
            get;
            set;
        } = -1;

        public override Command CommandToAdd => new Command("add", File);

        #region Style intergration

        public bool IsPlaying
        {
            get => m_IsPlaying;
            set
            {
                if (m_IsPlaying != value)
                {
                    m_IsPlaying = value;
                    NotifyPropertyChanged("IsPlaying");
                }
            }
        }

        public bool IsPaused
        {
            get => m_IsPaused;
            set
            {
                if (m_IsPaused != value)
                {
                    m_IsPaused = value;
                    NotifyPropertyChanged("IsPaused");
                }
            }
        }

        public bool IsSelected
        {
            get => m_IsSelected;
            set
            {
                if (m_IsSelected != value)
                {
                    m_IsSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsAboveDropTarget
        {
            get => m_AboveIsDropTarget;
            set
            {
                if (m_AboveIsDropTarget != value)
                {
                    m_AboveIsDropTarget = value;
                    NotifyPropertyChanged("IsAboveDropTarget");
                }
            }
        }

        public bool IsBelowDropTarget
        {
            get => m_BelowIsDropTarget;
            set
            {
                if (m_BelowIsDropTarget != value)
                {
                    m_BelowIsDropTarget = value;
                    NotifyPropertyChanged("IsBelowDropTarget");
                }
            }
        }

        public bool IsFirstItemInList
        {
            get => Pos == 0;
        }

        bool m_IsPlaying = false;
        bool m_IsPaused = false;
        bool m_IsSelected = false;
        bool m_AboveIsDropTarget = false;
        bool m_BelowIsDropTarget = false;

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
