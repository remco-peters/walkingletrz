using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class PlayerData
    {
        public string Name;
        public bool localPlayer;
        public long Points;
        public int place;
        public List<string> BestWords = null;
        public int WordCount = 0;
    }
}
