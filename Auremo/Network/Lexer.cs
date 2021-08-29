using System.IO;
using System.Text;

namespace Auremo.Network
{
    public class Lexer
    {
        private readonly LineSource m_LineSource = null;
        private StringReader m_LineReader = null;
        private char m_Next;
        private bool m_Eol;

        public enum LineType
        {
            Banner,
            Data,
            ResponseStatus,
            Unknown
        };

        public Lexer(LineSource source)
        {
            m_LineSource = source;
        }

        public void ReadLine()
        {
            CurrentLine = m_LineSource.ReadLine();
            CurrentBinaryChunk = null;

            m_LineReader = new StringReader(CurrentLine);
            m_Eol = false;
            Advance();
            TokenizeLine();
        }

        public void ReadBinaryChunk(int length)
        {
            CurrentLine = null;
            CurrentBinaryChunk = m_LineSource.ReadBytes(length);

            if (m_LineSource.ReadLine() != "")
            {
                throw new Exception("Expected newline after binary data");
            }

            m_Eol = false;
            Advance();
        }

        public string CurrentLine
        {
            get;
            private set;
        } = null;

        public LineType CurrentLineType
        {
            get;
            private set;
        } = LineType.Unknown;

        public byte[] CurrentBinaryChunk
        {
            get;
            private set;
        }

        public int MajorVersion
        {
            get;
            private set;
        }

        public int MinorVersion
        {
            get;
            private set;
        }

        public int PatchVersion
        {
            get;
            private set;
        }

        public bool IsOk
        {
            get;
            private set;
        }

        public bool IsAck => !IsOk;

        public int AckError
        {
            get;
            private set;
        }

        public int AckCommandListNumber
        {
            get;
            private set;
        }

        public string FailedCommand
        {
            get;
            private set;
        }

        public string AckMessage
        {
            get;
            private set;
        }

        public string DatumKey
        {
            get;
            private set;
        }

        public string DatumValue
        {
            get;
            private set;
        }

        public byte[] BinaryObject
        {
            get;
            private set;
        }

        private void TokenizeLine()
        {
            CurrentLineType = LineType.Unknown;
            string word = AcceptWord();

            if (word == "OK")
            {
                if (m_Eol)
                {
                    IsOk = true;
                    CurrentLineType = LineType.ResponseStatus;
                }
                else
                {
                    AcceptLiteral(" MPD ");
                    MajorVersion = AcceptInteger();
                    AcceptLiteral(".");
                    MinorVersion = AcceptInteger();
                    AcceptLiteral(".");
                    PatchVersion = AcceptInteger();
                    RequireEol();
                    CurrentLineType = LineType.Banner;
                }
            }
            else if (word == "ACK")
            {
                IsOk = false;
                AcceptLiteral(" [");
                AckError = AcceptInteger();
                AcceptLiteral('@');
                AckCommandListNumber = AcceptInteger();
                AcceptLiteral("] {");
                FailedCommand = AcceptUntilAndIncluding('}');
                AcceptLiteral(" ");
                AckMessage = AcceptRestOfLine();
                CurrentLineType = LineType.ResponseStatus;
            }
            // TODO: try putting the "binary:" here.
            else
            {
                AcceptLiteral(": ");
                DatumKey = word;
                DatumValue = AcceptRestOfLine();
                CurrentLineType = LineType.Data;
            }
        }

        private void Advance()
        {
            if (!m_Eol)
            {
                int next = m_LineReader.Read();
                m_Eol = next == -1;
                m_Next = (char)next;
            }
        }

        private void RequireEol()
        {
            if (!m_Eol)
            {
                throw new Exception("Expected EOL");
            }
        }

        private void AcceptLiteral(char c)
        {
            if (m_Next != c)
            {
                throw new Exception("Expected \'" + c + "\'");
            }

            Advance();
        }

        private void AcceptLiteral(string s)
        {
            try
            {
                foreach (char c in s)
                {
                    AcceptLiteral(c);
                }
            }
            catch (Exception)
            {
                throw new Exception("Expected \"" + s + "\"");
            }
        }

        private string AcceptWord()
        {
            StringBuilder result = new StringBuilder();

            // TODO: read until ':' might be fine here
            while (!m_Eol && char.IsLetter(m_Next) || m_Next == '-' || m_Next == '_')
            {
                result.Append(m_Next);
                Advance();
            }

            if (result.Length == 0)
            {
                throw new Exception("Expected a word");
            }

            return result.ToString();
        }

        public int AcceptInteger()
        {
            int result = 0;
            int digitsConsumed = 0;
            int sign = 1;

            if (!m_Eol && m_Next == '-')
            {
                sign = -1;
                Advance();
            }

            while (!m_Eol && char.IsDigit(m_Next))
            {
                result = 10 * result + (m_Next - '0');
                Advance();
                digitsConsumed += 1;
            }

            if (digitsConsumed == 0)
            {
                throw new Exception("Expected a number");
            }

            return sign * result;
        }

        private string AcceptUntilAndIncluding(char c)
        {
            StringBuilder result = new StringBuilder();

            while (m_Next != c)
            {
                result.Append(m_Next);
                Advance();
            }

            Advance();
            return result.ToString();
        }

        private string AcceptRestOfLine()
        {
            if (m_Eol)
            {
                return "";
            }
            else
            {
                m_Eol = true;
                return m_Next + m_LineReader.ReadToEnd();
            }
        }
        
        #region Exceptions

        public class Exception : System.Exception
        {
            public Exception(string message) :
                base("Network.Lexer exception: " + message)
            {
            }
        }

        #endregion
    }
}
