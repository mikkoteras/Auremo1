using Auremo.Network;
using System.ComponentModel;

namespace Auremo.DataModel.AudioObjects
{
    public class Artist : AudioObject
    {
        public Artist(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public override Command CommandToAdd => new Command("findadd", $"(artist == '{Name}')");
    }
}
