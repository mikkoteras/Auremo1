using System;
using System.Globalization;
using System.Text;

namespace Auremo.Network
{
    public class Command : Sendable
    {
        public Command(string verb)
        {
            Verb = verb;
            Arguments = null;
            Sendable = verb + "\n";
        }

        public Command(string verb, params object[] arguments)
        {
            Verb = verb;
            Arguments = arguments;
            StringBuilder fullCommand = new StringBuilder();
            fullCommand.Append(verb);

            foreach (object arg in arguments)
            {
                fullCommand.Append(' ');
                string argument = arg is double d ? d.ToString(CultureInfo.InvariantCulture) : arg.ToString();
                fullCommand.Append(Quote(argument));
            }

            fullCommand.Append('\n');
            Sendable = fullCommand.ToString();
        }

        public string Verb
        {
            get;
            private set;
        }

        public object[] Arguments
        {
            get;
            private set;
        }

        public string Sendable
        {
            get;
            private set;
        }

        private static string Quote(string s)
        {
            string withEscapedBackslashes = s.Replace("\\", "\\\\");
            string withEscapedEverything = withEscapedBackslashes.Replace("\"", "\\\"");
            return '\"' + withEscapedEverything + '\"';
        }

        public override string ToString()
        {
            return Sendable.TrimEnd('\n');
        }
    }
}
