using Auremo.Network;
using System;
using System.Text;
using System.Windows;

namespace Auremo.DataModel
{
    public class CommandRunner : ServerConnectorBase
    {
        // TODO: split this into two: a command runner that doesn't need
        // binary support, and an album art fetcher that does.
        public CommandRunner() : base(true, "Command runner")
        {
        }

        public void PushCommand(Sendable request)
        {
            // TODO: what if the connection is down?
            m_Connection.Send(request, OnCommandResponseReceived);
        }

        public void PushQuery(Command command, Connection.ResponseCallback callback)
        {
            m_Connection.Send(command, callback);
        }

        private void OnCommandResponseReceived(Sendable request, Response response)
        {
            Application.Current.Dispatcher.BeginInvoke((Connection.ResponseCallback)ProcessCommandResponse, request, response);
        }

        private void ProcessCommandResponse(Sendable request, Response response)
        {
            if (response.IsAck)
            {
                StringBuilder message = new StringBuilder();
                message.Append("MPD command failed. Command: \"");
                message.Append(request.ToString());
                message.Append("\" Response: \"");
                message.Append(response.AckMessage);
                message.Append("\"");
                World.Instance.Log.LogMessageFromThread(message.ToString());
            }
            else if (response.HasData)
            {
                // TODO: exception, seriously?
                throw new Exception("CommandRunner.ReadCommandResponse(): logic error: unexpected data in response.");
            }
        }
    }
}
