using Auremo.Network;

namespace Auremo.DataModel.AudioObjects
{
    public abstract class AudioObject
    {
        public abstract Command CommandToAdd { get; }
    }
}
