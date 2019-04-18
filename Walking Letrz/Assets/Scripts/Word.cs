namespace Assets.Scripts
{
    public class Word
    {
        public string word;
        public long points;

        public Word(string word, long points)
        {
            this.points = points;
            this.word = word;
        }
    }
}
