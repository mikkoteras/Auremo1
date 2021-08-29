using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Auremo.DataModel
{
    public class Log : INotifyPropertyChanged
    {
        public Log()
        {
            Entries = new ObservableCollection<string>();
        }

        public IList<string> Entries
        {
            get;
            private set;
        }

        public void LogMessage(string message)
        {
            if (IsPaused)
            {
                m_Spool.Add(message);
            }
            else
            {
                while (Entries.Count >= m_DisplayedLogSize)
                {
                    Entries.RemoveAt(0);
                }

                Entries.Add(message);
            }
        }

        private delegate void LogMessageDelegate(string message);

        public void LogMessageFromThread(string message)
        {
            Application.Current.Dispatcher.BeginInvoke((LogMessageDelegate)LogMessage, message);
        }

        public bool IsPaused
        {
            get => m_IsPaused;
            set
            {
                if (m_IsPaused != value)
                {
                    m_IsPaused = value;

                    if (!m_IsPaused)
                    {
                        foreach (string message in m_Spool)
                        {
                            LogMessage(message);
                        }

                        m_Spool.Clear();
                    }

                    NotifyPropertyChanged("IsPaused");
                }
            }
        }

        public void Clear()
        {
            Entries.Clear();
            m_Spool.Clear();
        }

        private const int m_DisplayedLogSize = 1000;
        private bool m_IsPaused = false;
        private IList<string> m_Spool = new List<string>(); // Store incoming messages here while paused

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
