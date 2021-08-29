using Auremo.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auremo.DataModel.AudioObjects
{
    public class Track : AudioObject
    {
        public Track()
        {
        }

        public string File
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string TitleOrFile => Title ?? File;

        public string Artist
        {
            get;
            set;
        }

        public string Album
        {
            get;
            set;
        }

        public string Date
        {
            get;
            set;
        }

        public override Command CommandToAdd => new Command("add", File);
    }
}
