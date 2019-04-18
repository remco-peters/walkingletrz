using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MyMonoBehaviour
    {
        public float TimeRemaining { get; set; }
        public long EarnedPoints { get; set; }
        public bool CanMove { get; set; }
        public LetterManager LetterManager { get; set; }
        public string Name { get; set; }
        public TheLetterManager TheLetterManager { get; set; }
        public List<Word> BestWordsThisGame{get; set;} = new List<Word>();

        public void Start()
        {
            TimeRemaining = 120;
            EarnedPoints = 0;
        }

        public void Update()
        {
            if (!CanMove) return;
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0)
            {
                CanMove = false;
            }
        }
    }
}
