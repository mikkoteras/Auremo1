using Auremo.DataModel.DataStores;
using Auremo.DataModel.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Auremo.DataModel
{
    public class ViewHistory : INotifyPropertyChanged
    {
        private readonly List<ViewDefinition> m_History = new List<ViewDefinition>();
        private int m_CurrentPosition = -1; // The current index within the history list

        public ViewHistory()
        {
            // This is never null at runtime, but will be for Designer.
            if (World.Instance != null)
            {
                World.Instance.ServerStatus.PropertyChanged += OnServerStatusPropertyChanged;
            }
        }

        public void Reset()
        {
            m_History.Clear();
            m_CurrentPosition = -1;
            World.Instance.InterfaceState.DisplayView(ViewMode.Activity);
            SignalUpdate();
        }

        public void Restart()
        {
            m_CurrentPosition = 0;
            SwitchToCurrentView();
            SignalUpdate();
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                m_CurrentPosition -= 1;
                SwitchToCurrentView();
                SignalUpdate();
            }
        }

        public void GoForward()
        {
            if (CanGoForward)
            {
                m_CurrentPosition += 1;
                SwitchToCurrentView();
                SignalUpdate();
            }
        }

        public void PushView(ViewDefinition definition)
        {
            m_CurrentPosition += 1;

            if (m_CurrentPosition < m_History.Count)
            {
                m_History.RemoveRange(m_CurrentPosition, m_History.Count - m_CurrentPosition);
            }

            m_History.Add(definition);
            SignalUpdate();
        }

        public bool CanGoBack => m_CurrentPosition > 0;

        public bool CanGoForward => m_CurrentPosition < m_History.Count - 1;

        public void SwitchToCurrentView()
        {
            ViewDefinition definition = m_History[m_CurrentPosition];

            if (definition.Creator == null)
            {
                World.Instance.InterfaceState.ViewMode = definition.ViewMode;
            }
            else
            {
                definition.Creator.RestoreFromDefinition(definition);
            }
        }

        private void SignalUpdate()
        {
            NotifyPropertyChanged("CanGoBack");
            NotifyPropertyChanged("CanGoForward");
            
        }

        private void OnServerStatusPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsConnected")
            {
                Reset();
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
