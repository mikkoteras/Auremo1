namespace Auremo.DataModel
{
    public class Output
    {
        public Output()
        {
            Id = -1;
            Name = null;
            Enabled = false;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }
    }
}
