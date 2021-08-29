using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auremo.Network
{
    // Provides an abstract base for reading lines from the stream
    // and feeding them to the Lexer. Unfortunately, there is no
    // single implementation that works in all cases.
    public abstract class LineSource
    {
        public abstract string ReadLine();
        public abstract byte[] ReadBytes(int count);

        public class ReadCanceledException : Exception { }
    }
}
