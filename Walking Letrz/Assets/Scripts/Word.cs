using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public struct Word
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
