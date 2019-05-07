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

        public static bool joinedRoom = false;

        public void Awake()
        {
            TimeRemaining = GameInstance.GetGameTimeInSeconds();
            EarnedPoints = 0;
            
        }

        public void Start()
        {
            
        }

        public void Update()
        {
            if (!joinedRoom) return;
            if (!CanMove) return;
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0)
            {
                //CanMove = false;
            }
        }
    }
}
