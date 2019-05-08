using System.Collections.Generic;

namespace Assets.Scripts
{
    public class PlayerData
    {
        public string Name;
        public bool localPlayer;
        public long Points;
        public long PointsWithoutTime;
        public int place;
        public List<string> BestWords = null;
        public int WordCount = 0;
        public float timeLeft;
        public int WordCountTwelveLetters = 0;
        public int FinalWordCountPerMinute = 0;
    }
}
