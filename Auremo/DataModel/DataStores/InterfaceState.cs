using Auremo.DataModel.DataStores;
using Auremo.DataModel.Types;
using System;
using System.ComponentModel;

namespace Auremo.DataModel
{
    public class InterfaceState : DataStores.RestorableDataStore, INotifyPropertyChanged
    {
        public InterfaceState()
        {
        }

        public ViewMode ViewMode
        {
            get => m_ViewMode;
            set
            {
                if (m_ViewMode != value)
                {
                    m_ViewMode = value;
                    NotifyPropertyChanged("ViewMode");
                    NotifyPropertyChanged("ViewModeIsActivity");
                    NotifyPropertyChanged("ViewModeIsQueryResult");
                    NotifyPropertyChanged("ViewModeIsPlaylistSelector");
                    NotifyPropertyChanged("ViewModeIsPlaylistContents");
                    NotifyPropertyChanged("ViewModeIsPlayQueue");
                    NotifyPropertyChanged("ViewModeIsServer");
                    NotifyPropertyChanged("ViewModeIsConfiguration");
                    NotifyPropertyChanged("ViewModeIsDeveloper");
                    NotifyPropertyChanged("ViewModeIsSandbox");
                    NotifyPropertyChanged("ViewModeIsQueryResultOrPlaylistSelector");
                    NotifyPropertyChanged("ViewModeIsPlaylistSelectorOrContents");
                }
            }
        }

        public string SearchString
        {
            get => m_SearchString;
            set
            {
                if (m_SearchString != value)
                {
                    m_SearchString = value;
                    NotifyPropertyChanged("SearchString");
                }
            }
        }


        public void DisplayView(ViewMode view)
        {
            ViewMode = view;
            World.Instance.ViewHistory.PushView(new ViewDefinition(view, this));
        }

        public override void RestoreFromDefinition(ViewDefinition definition)
        {
            if (definition.Creator == this)
            {
                ViewMode = definition.ViewMode;
            }
            else
            {
                throw new Exception("InterfaceState.RestoreFromDefinition(): Logic error!");
            }
        }

        public bool ViewModeIsActivity => ViewMode == ViewMode.Activity;
        public bool ViewModeIsQueryResult => ViewMode == ViewMode.QueryResult;
        public bool ViewModeIsPlaylistSelector => ViewMode == ViewMode.PlaylistSelector;
        public bool ViewModeIsPlaylistContents => ViewMode == ViewMode.PlaylistContents;
        public bool ViewModeIsPlayQueue => ViewMode == ViewMode.PlayQueue;
        public bool ViewModeIsServer => ViewMode == ViewMode.Server;
        public bool ViewModeIsConfiguration => ViewMode == ViewMode.Configuration;
        public bool ViewModeIsDeveloper => ViewMode == ViewMode.Developer;
        public bool ViewModeIsSandbox => ViewMode == ViewMode.Sandbox;

        // Needed to recycle QueryResult. Maybe it's a stupid idea.
        public bool ViewModeIsQueryResultOrPlaylistSelector => ViewMode == ViewMode.QueryResult || ViewMode == ViewMode.PlaylistSelector;
        public bool ViewModeIsPlaylistSelectorOrContents => ViewMode == ViewMode.PlaylistSelector|| ViewMode == ViewMode.PlaylistContents;

        public bool PopupOverlayIsActive
        {
            get => m_PopupOverlayIsActive;
            set
            {
                if (m_PopupOverlayIsActive != value)
                {
                    m_PopupOverlayIsActive = value;
                    NotifyPropertyChanged("PopupOverlayIsActive");
                }
            }
        }

        private ViewMode m_ViewMode = ViewMode.Activity;
        private string m_SearchString = "";
        private bool m_PopupOverlayIsActive = false;

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
