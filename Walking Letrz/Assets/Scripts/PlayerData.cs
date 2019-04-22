using System.Collections.Generic;

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
        public float timeLeft;
    }
}
