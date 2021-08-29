using Auremo.Network;

namespace Auremo.DataModel
{
    public class QueryRunner : ServerConnectorBase
    {
        #warning TODO this class is now really stupid and fit for refactoring.

        public QueryRunner() : base(false, "Query runner")
        {
        }

        public void PushQuery(Sendable query, Connection.ResponseCallback callback)
        {
            m_Connection.Send(query, callback);
        }
    }
}
