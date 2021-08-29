using Auremo.DataModel;
using Auremo.Network;
using System;
using System.ComponentModel;

namespace Auremo.Controls.Utility
{
    /// <summary>
    /// A property stored on the MPD server, but settable with a UI widget. A
    /// standard property won't work here, because there are two competing
    /// parties setting the value and causing updates to the other party.
    /// Also, because the setting the value at the server requires a network
    /// message, and because widgets like sliders can cause several value
    /// updates per second, using a normal propery could lead to a message storm
    /// that hoses the server.
    /// </summary>
    public class RemoteProperty<T> : INotifyPropertyChanged where T : struct, IComparable
    {
        public delegate Sendable SetValue(T arg);

        public RemoteProperty(T val, SetValue setValue)
        {
            m_DefaultValue = val;
            m_ClientSideValue = val;
            m_SpooledValue = null;
            m_ServerSideValue = val;
            m_SetValue = setValue;
        }

        public T ClientSideValue
        {
            get
            {
                return m_ClientSideValue;
            }
            set
            {
                if (m_ClientSideValue.CompareTo(value) != 0)
                {
                    if (UserIsEditing)
                    {
                        if (m_SpooledValue.HasValue)
                        {
                            m_SpooledValue = value;
                        }
                        else
                        {
                            World.Instance.CommandRunner.PushCommand(m_SetValue(value));
                        }
                    }

                    m_ClientSideValue = value;
                    NotifyPropertyChanged("ClientSideValue");
                }
            }
        }

        public T? ServerSideValue
        {
            get
            {
                return m_ServerSideValue;
            }
            set
            {
                if (m_ServerSideValue.HasValue != value.HasValue ||
                    m_ServerSideValue.HasValue && value.HasValue && m_ServerSideValue.Value.CompareTo(value.Value) != 0)
                {
                    m_ServerSideValue = value;

                    if (m_SpooledValue.HasValue)
                    {
                        World.Instance.CommandRunner.PushCommand(m_SetValue(m_SpooledValue.Value));
                        m_SpooledValue = null;
                    }

                    if (!UserIsEditing)
                    {
                        ClientSideValue = value.GetValueOrDefault(m_DefaultValue);
                    }

                    NotifyPropertyChanged("ServerSideValue");
                }
            }
        }

        public bool UserIsEditing
        {
            get
            {
                return m_UserIsEditing;
            }
            set
            {
                m_UserIsEditing = value;

                if (!UserIsEditing)
                {
                    ClientSideValue = ServerSideValue.GetValueOrDefault(m_DefaultValue);
                }
            }
        }

        private readonly T m_DefaultValue;
        private T m_ClientSideValue;
        private T? m_ServerSideValue;
        private T? m_SpooledValue; // Value stored for sending when when current roundtrip completes.
        private bool m_UserIsEditing = false;
        private readonly SetValue m_SetValue;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
