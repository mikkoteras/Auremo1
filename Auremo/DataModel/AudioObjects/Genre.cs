using Auremo.Network;

namespace Auremo.DataModel.AudioObjects
{
    public class Genre : AudioObject
    {
        public Genre(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }

        public override Command CommandToAdd => new Command("findadd", $"(genre == '{Name}')");
    }
}
