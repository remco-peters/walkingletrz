using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerManager : MyMonoBehaviour
    {
        public List<Player> Players { get; set; }
        
        public void NextTurn()
        {
            int index = Players.IndexOf(GetCurrentActivePlayer());
            if (index == -1) return;
            index += 1;
            if (index >= Players.Count)
                index = 0;
            Player newPlayer = Players[index];
            GetCurrentActivePlayer().CanMove = false;
            newPlayer.CanMove = true;
        }

        public Player GetCurrentActivePlayer()
        {
            return Players.FirstOrDefault(x => x.CanMove);
        }

        public void ChangeWalkingLetters(char FirstLetter, char SecondLetter)
        {
            foreach (Player player in Players)
            {
                //todo somehow get the walkingletterz of the player
                LetterBlock firstLetter = player.LetterManager.InstantiateLetterButton(FirstLetter, new Vector3(), true, false, 0, 0);
                LetterBlock secondLetter = player.LetterManager.InstantiateLetterButton(SecondLetter, new Vector3(), false, true, 0, 1);
            }
        }
    }
}
