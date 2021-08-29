using Auremo.DataModel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auremo.Utility
{
    public static class ParserUtility
    {
        public static SingleMode StringToSingleMode(string text)
        {
            if (text == "0")
            {
                return SingleMode.False;
            }
            else if (text == "1")
            {
                return SingleMode.True;
            }
            else if (text == "oneshot")
            {
                return SingleMode.True;
            }
            else
            {
                return SingleMode.Undefined;
            }
        }

        public static PlayState StringToPlayState(string text)
        {
            if (text == "stop")
            {
                return PlayState.Stopped;
            }
            else if (text == "pause")
            {
                return PlayState.Paused;
            }
            else if (text == "play")
            {
                return PlayState.Playing;
            }
            else
            {
                return PlayState.Unknown;
            }
        }
    }
}
