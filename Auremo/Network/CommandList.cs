using System.Collections.Generic;
using System.Text;

namespace Auremo.Network
{
    public class CommandList : Sendable
    {
        public CommandList()
        {
            Commands = new List<Command>();
        }

        public CommandList(params Command[] commands)
        {
            Commands = new List<Command>(commands);
        }

        public IList<Command> Commands
        {
            get;
            private set;
        }

        public string Sendable
        {
            get
            {
                StringBuilder result = new StringBuilder();
                result.Append("command_list_begin\n");

                foreach (Command command in Commands)
                {
                    result.Append(command.Sendable);
                }

                result.Append("command_list_end\n");
                return result.ToString();
            }
        }

        public override string ToString()
        {
            return Sendable.TrimEnd('\n');
        }
    }
}
