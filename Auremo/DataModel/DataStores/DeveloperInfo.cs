using Auremo.Controls.Utility;
using Auremo.Network;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Auremo.DataModel.DataStores
{
    public class DeveloperInfo : INotifyPropertyChanged
    {
        public DeveloperInfo()
        {
            Response = new ObservableCollection<string>();
        }

        public void SendRequest()
        {
            Response.Clear();
            #warning TODO do not use QueryRunner -- always create a new connection specifically for this.
            World.Instance.QueryRunner.PushQuery(new Command(Request), OnResponseReceived);
        }

        private void OnResponseReceived(Sendable request, Response response)
        {
            ThreadUtility.RunInUiThread(() =>
            {
                if (response.HasData)
                {
                    foreach (Datum datum in response.Data)
                    {
                        Response.Add(datum.ToString());
                    }
                }

                Response.Add(response.StatusLine);
            });
        }

        public string Request
        {
            get => m_Request;
            set
            {
                if (m_Request != value)
                {
                    m_Request = value;
                    NotifyPropertyChanged("Request");
                }
            }
        }

        public ObservableCollection<string> Response
        {
            get;
            private set;
        }

        private string m_Request = "";

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
