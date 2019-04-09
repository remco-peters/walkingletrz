using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MyBot : Player
    {
        // Start is called before the first frame update
        public float timeRemaining;
        public PlayerManager playerManager;
        private char[] Letters;

        new void Start()
        {
            Letters = LetterManager.GetLetters(15);
            timeRemaining = Random.Range(2, 10);
            base.Start();
        }

        // Update is called once per frame
        new void Update()
        {
            if (CanMove)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    //TODO MAKE WORD WITH MY LETTERS AND PLACE IN GAME
                    EarnedPoints += 10;
                    playerManager.NextTurn(this);
                    timeRemaining = Random.Range(2, 10);
                    PlacedInBoard("yoghurt");
                }
            }
            base.Update(); 
        }

        public void PlacedInBoard(string word)
        {
            TheLetterManager.PlacedWords.Add(word);
            TheLetterManager.PlaceWordInGameBoard(word.ToCharArray());
        }
    }
}
