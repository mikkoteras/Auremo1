using Auremo.Network;

namespace Auremo.DataModel.AudioObjects
{
    public class Playlist : AudioObject
    {
        public Playlist(string file)
        {
            File = file;
        }

        public string File
        {
            get;
            set;
        }

        public override Command CommandToAdd => new Command("load", File);

        public override string ToString() { return File; }
    }
}
