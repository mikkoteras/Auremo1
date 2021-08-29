using System;
using System.IO;
using System.Text;

namespace Auremo.Network
{
    // This is a version of LineSource that implements ReadBytes(),
    // but doesn't support canceling with CancellationToken. It also
    // probably performs much worse that a TextReader based solution,
    // so this is to be used only when binary responses are expected.
    public class BinaryLineSource : LineSource
    {
        private BinaryReader m_Reader = null;

        public BinaryLineSource(Stream stream)
        {
            m_Reader = new BinaryReader(stream, Encoding.UTF8);
        }

        public override string ReadLine()
        {
            StringBuilder result = new StringBuilder();
            char c = m_Reader.ReadChar();

            while (c != '\n')
            {
                result.Append(c);
                c = m_Reader.ReadChar();
            }

            return result.ToString();
        }

        public override byte[] ReadBytes(int count)
        {
            byte[] result = m_Reader.ReadBytes(count);

            if (result.Length != count)
            {
                throw new Exception("BinaryLineSource.ReadBytes(): stream ended unexpectedly.");
            }

            return result;
        }
    }
}
