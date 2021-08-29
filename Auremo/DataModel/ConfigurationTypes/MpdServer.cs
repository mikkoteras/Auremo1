using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Auremo.DataModel.ConfigurationObjects
{
    public class MpdServer : INotifyPropertyChanged
    {
        public MpdServer()
        {
            Host = "localhost";
            Port = 6600;
            Password = "";
        }

        public MpdServer(MpdServer rhs)
        {
            Host = rhs.Host;
            Port = rhs.Port;
            Password = rhs.Password;
        }

        [XmlAttribute]
        public string Label
        {
            get => m_Label;
            set
            {
                if (m_Label != value)
                {
                    m_Label = value;
                    NotifyPropertyChanged("Label");
                }
            }
        }

        [XmlAttribute]
        public string Host
        {
            get => m_Host;
            set
            {
                if (m_Host != value)
                {
                    m_Host = value;
                    NotifyPropertyChanged("Host");
                }
            }
        }

        [XmlAttribute]
        public int Port
        {
            get => m_Port;
            set
            {
                if (m_Port != value)
                {
                    m_Port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }

        [XmlAttribute]
        public string Password
        {
            get => m_Password;
            set
            {
                if (m_Password != value)
                {
                    m_Password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        public override string ToString()
        {
            return Host + ":" + Port + ":" + Password;
        }

        private string m_Label;
        private string m_Host;
        private int m_Port;
        private string m_Password;

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
