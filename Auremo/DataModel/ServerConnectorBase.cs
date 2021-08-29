using Auremo.Network;
using System.ComponentModel;
using System.Threading;

namespace Auremo.DataModel
{
    // A basic implementation of an application component that runs in
    // a background thread and utilizes a connection to the MPD host.
    public abstract class ServerConnectorBase : INotifyPropertyChanged
    {
        private readonly bool m_SupportBinary;
        private string m_Description = null;
        private bool m_Stopping = false;
        private Thread m_Thread = null;
        private volatile bool m_ResetConnection = false;
        protected Connection m_Connection = null;

        protected ServerConnectorBase(bool supportBinary, string description)
        {
            m_SupportBinary = supportBinary;
            m_Description = description;
            m_Thread = new Thread(Run) { Name = m_Description };
            m_Thread.Start();
        }

        public void Stop()
        {
            m_Stopping = true;
        }

        public void Join()
        {
            m_Thread.Join();
        }

        // Functions implementable by child classes.
        // All are called from the child thread's context.

        // Called once at the start of thread execution.
        protected virtual void OnEnterThread() { }

        // Called once at the end of thread execution.
        protected virtual void OnExitThread() { }

        // Called every time a connection has been successfully
        // established to an MPD host.
        protected virtual void OnConnected() { }

        // Called every time a connection that had been
        // established to an MPD host terminates. Always
        // preceded by an OnConnected() call.
        protected virtual void OnDisconnected() { }

        // Called in a loop while executing and connected.
        // The child class may return false to signal that it
        // wants the thread to terminate its connection to the
        // MPD host and exit. By default, just sit there.
        protected virtual bool Work()
        {
            Thread.Sleep(100);
            return true;
        }

        private void Run()
        {
            OnEnterThread();
            World.Instance.Configuration.PropertyChanged += OnConfigurationChanged;

            while (!m_Stopping)
            {
                m_ResetConnection = false;
                m_Connection = new Connection(World.Instance.Configuration.ActiveOptions.PrimaryServer, m_SupportBinary, m_Description);

                while (!m_Stopping && !m_ResetConnection && m_Connection.State == Connection.ConnectionState.Connecting)
                {
                    Thread.Sleep(100);
                }

                bool wasConnected = false;

                if (m_Connection.State == Connection.ConnectionState.Connected)
                {
                    wasConnected = true;
                    OnConnected();
                }

                bool subclassContinues = true;

                while (m_Connection.State == Connection.ConnectionState.Connected && subclassContinues && !m_Stopping && !m_ResetConnection)
                {
                    subclassContinues = Work();
                }

                m_Connection.Join();

                if (wasConnected)
                {
                    OnDisconnected();
                }

                m_Connection = null;
            }

            OnExitThread();
        }

        private void OnConfigurationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveOptions")
            {
                m_ResetConnection = true;
            }
        }

        #region INotifyPropertyChanged, protected version

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
