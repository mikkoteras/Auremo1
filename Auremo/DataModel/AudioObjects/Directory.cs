using Auremo.Network;
using System.ComponentModel;

namespace Auremo.DataModel.AudioObjects
{
    public class Directory : AudioObject
    {
        public Directory(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public override Command CommandToAdd => new Command("add", Name);
    }
}
