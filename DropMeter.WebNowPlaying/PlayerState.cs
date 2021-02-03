using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropMeter.WebNowPlaying
{
    public class PlayerState
    {
        public int STATE; //T-1 - 2-Stop, 1-Play
        public string TITLE;
        public string ARTIST;
        public string ALBUM;
        public string DURATION; //0:00
        public string POSITION; //0:00
        public int VOLUME;
        public int RATING;
        public int REPEAT; //0|1|2
        public bool SHUFFLE;
        public string COVER;
    }
}
