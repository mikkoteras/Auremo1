using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auremo.Network
{
    // This is a version of LineSource that is high performance and
    // supports canceling, but does not support binary chunks. It
    // is to be used as the normal line source for MPD connections
    // that don't need binary responses.
    public class TextLineSource : LineSource
    {
        private StreamReader m_Reader = null;
        private CancellationToken m_Cancel;

        public TextLineSource(Stream stream, CancellationTokenSource canceler)
        {
            m_Reader = new StreamReader(stream, Encoding.UTF8);
            m_Cancel = canceler.Token;
        }

        public override string ReadLine()
        {
            Task<string> task = m_Reader.ReadLineAsync();
            task.Wait(m_Cancel);

            if (!task.IsCompleted)
            {
                throw new ReadCanceledException();
            }

            return task.Result;
        }

        public override byte[] ReadBytes(int count)
        {
            throw new Exception("TextLineSource.ReadBytes(): unexpected method invokation.");
        }
    }
}
