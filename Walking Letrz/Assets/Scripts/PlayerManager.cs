using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerManager : MyMonoBehaviour
    {
        public List<Player> Players { get; set; }
        
        /// <summary>
        /// Method that will be executed when the next player needs to get the turn
        /// </summary>
        public void NextTurn()
        {
            int index;
            if(GameInstance.instance.IsMultiplayer)
            {
                var currentPlayer = GetCurrentActivePlayer();
                index = Array.IndexOf(PhotonNetwork.PlayerList, currentPlayer);

                if (index == -1) return;

                index += 1;
                if (index == PhotonNetwork.PlayerList.Length)
                    index = 0;

                Photon.Realtime.Player newPlayer = PhotonNetwork.PlayerList[index];
                Hashtable hash = new Hashtable { { "CanMove", true } };
                newPlayer.SetCustomProperties(hash);

                Hashtable hashh = new Hashtable { { "CanMove", false } };
                currentPlayer.SetCustomProperties(hashh);
            }
            else
            {
                index = Players.IndexOf(GetCurrentActivePlayerSinglePlayer());
                if (index == -1) return;
                index += 1;
                if (index >= Players.Count)
                    index = 0;
                Player newPlayer = Players[index];
                GetCurrentActivePlayerSinglePlayer().CanMove = false;
                newPlayer.CanMove = true;

            }
            
        }

        /// <summary>
        /// Gets the players that can move from photon
        /// </summary>
        /// <returns></returns>
        public Photon.Realtime.Player GetCurrentActivePlayer()
        {
            return PhotonNetwork.PlayerList.FirstOrDefault(x => (bool) x.CustomProperties["CanMove"]);
        }

        private Player GetCurrentActivePlayerSinglePlayer()
        {
            return Players.FirstOrDefault(x => x.CanMove);
        }

        public void ChangeWalkingLetters(char FirstLetter, char SecondLetter)
        {
            foreach (Player player in Players)
            {
                LetterBlock firstLetter = player.LetterManager.InstantiateLetterButton(FirstLetter, true, false, 0, 0);
                LetterBlock secondLetter = player.LetterManager.InstantiateLetterButton(SecondLetter, false, true, 0, 1);
            }
        }
    }
}
