using Auremo.DataModel;
using Auremo.DataModel.ConfigurationObjects;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Auremo.Network
{
    public class Connection : INotifyPropertyChanged
    {
        public delegate void ResponseCallback(Sendable request, Response response);

        public enum ConnectionState
        {
            Connecting,
            Connected,
            Disconnected
        }

        private struct Task
        {
            public Sendable Request;
            public ResponseCallback Callback;
        }

        private readonly bool m_SupportBinary = false;
        private readonly string m_ServerLabel = "";
        private readonly string m_Host = "";
        private readonly int m_Port = -1;
        private readonly string m_Password = "";
        private readonly string m_Description = "";

        private Thread m_Thread = null;
        private ManualResetEvent m_Activity = new ManualResetEvent(false);
        private readonly CancellationTokenSource m_Canceler = new CancellationTokenSource();
        private readonly ConcurrentQueue<Task> m_RequestQueue = new ConcurrentQueue<Task>();
        private volatile bool m_Stopping = false;
        private volatile bool m_Stopped = false;

        public Connection(MpdServer server, bool supportBinary, string description)
        {
            // Copy all the attributes as MpdServer members are theoretically mutable.
            m_SupportBinary = supportBinary;
            m_ServerLabel = server.Label;
            m_Host = server.Host;
            m_Port = server.Port;
            m_Password = server.Password;
            m_Description = description;

            State = ConnectionState.Connecting;
            MpdBanner = "";

            m_Thread = new Thread(Run);
            m_Thread.Start();
        }

        public void Send(Sendable request)
        {
            Send(request, null);
        }

        public void Send(Sendable request, ResponseCallback callback)
        {
            Task task = new Task
            {
                Request = request,
                Callback = callback
            };
            m_RequestQueue.Enqueue(task);
            m_Activity.Set();
        }

        public void Stop()
        {
            m_Stopping = true;
            m_Activity.Set();
            m_Canceler.Cancel();
        }

        public void Join()
        {
            if (m_Thread != null)
            {
                if (!m_Stopping)
                {
                    Stop();
                }

                m_Thread.Join();
                m_Thread = null;
            }
        }

        public bool Stopped => m_Stopped;

        public ConnectionState State
        {
            get;
            private set;
        }

        public string MpdBanner
        {
            get;
            private set;
        }

        private void Run()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    if (Connect(client))
                    {
                        State = ConnectionState.Connected;
                        LineSource lineSource = m_SupportBinary
                            ? (LineSource)(new BinaryLineSource(client.GetStream()))
                            : (LineSource)(new TextLineSource(client.GetStream(), m_Canceler));
                        Lexer lexer = new Lexer(lineSource);
                        Parser parser = new Parser(lexer);
                        MpdBanner = parser.ParseBanner();
                        State = ConnectionState.Connected;

                        using (TextWriter writer = new StreamWriter(client.GetStream()))
                        {
                            ExecuteCommands(parser, writer);
                        }
                    }
                }
            }
            catch (LineSource.ReadCanceledException)
            {
                // We caused this. It's okay. Fall through.
            }
            catch (Exception e)
            {
                string label = m_ServerLabel == "" ? "" : " (" + m_ServerLabel + ")";
                string message = $"Connection to {m_Host}:{m_Port}{label} failed: {e.Message}";
                World.Instance.Log.LogMessageFromThread(message);
            }

            World.Instance.Log.LogMessageFromThread("Connection thread exiting.");
            State = ConnectionState.Disconnected;
            m_Stopped = true;
        }

        bool Connect(TcpClient client)
        {
            bool connected = false;

            while (!connected && !m_Stopping)
            {
                try
                {
                    IAsyncResult connectResult = client.BeginConnect(m_Host, m_Port, null, null);

                    while (!connectResult.IsCompleted && !m_Stopping)
                    {
                        Thread.Sleep(100);
                    }

                    if (!m_Stopping)
                    {
                        client.EndConnect(connectResult);
                        connected = true;
                    }
                }
                catch
                {
                }
            }

            return connected;
        }

        void ExecuteCommands(Parser parser, TextWriter writer)
        {
            while (!m_Stopping)
            {
                m_Activity.Reset();

                if (!m_Stopping && m_RequestQueue.IsEmpty)
                {
                    m_Activity.WaitOne(5000); // Do keepalive every 5 sec.
                }

                if (!m_Stopping)
                {
                    if (!m_RequestQueue.TryDequeue(out Task task))
                    {
                        // There is nothing else to send, so do keepalive.
                        task = new Task()
                        {
                            Request = new Command("ping"),
                            Callback = null
                        };
                    }

                    writer.Write(task.Request.Sendable);
                    writer.Flush();

                    Response response = parser.ParseResponse();

                    if (task.Callback != null)
                    {
                        task.Callback?.Invoke(task.Request, response);
                    }
                }
            }
        }
        
        public override string ToString()
        {
            return "Auremo.Network.Connection \"" + m_Description + "\"";
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #warning TODO what's this about? Used, not signaled, still works?
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
