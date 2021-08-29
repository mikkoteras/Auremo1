namespace Auremo.Network
{
    public class Parser
    {
        private readonly Lexer source = null;

        public Parser(Lexer lexer)
        {
            source = lexer;
        }

        public string ParseBanner()
        {
            source.ReadLine();

            if (source.CurrentLineType != Lexer.LineType.Banner)
            {
                throw new Exception("Expected MPD banner");
            }

            return source.CurrentLine;
        }

        public Response ParseResponse()
        {
            Response response = new Response();
            source.ReadLine();

            while (source.CurrentLineType == Lexer.LineType.Data)
            {
                Datum datum = new Datum
                {
                    Key = source.DatumKey,
                    Value = source.DatumValue
                };
                response.Data.Add(datum);

                if (source.DatumKey == "binary")
                {
                    source.ReadBinaryChunk(datum.IntValue());
                    response.BinaryObject = source.CurrentBinaryChunk;
                }

                source.ReadLine();
            }

            if (source.CurrentLineType == Lexer.LineType.ResponseStatus)
            {
                response.StatusLine = source.CurrentLine;
                response.IsOk = source.IsOk;
                response.AckError = source.AckError;
                response.AckCommandListNumber = source.AckCommandListNumber;
                response.FailedCommand = source.FailedCommand;
                response.AckMessage = source.AckMessage;
            }
            else
            {
                throw new Exception("Unexpected response");
            }

            return response;
        }
        
        #region Exceptions

        public class Exception : System.Exception
        {
            public Exception(string message) :
                base("Network.Parser exception: " + message)
            {
            }
        }

        #endregion
    }
}
