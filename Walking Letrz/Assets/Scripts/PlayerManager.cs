using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerManager : MyMonoBehaviour
    {
        public List<Player> Players { get; set; }

        private void Awake()
        {
//            PhotonManager.PhotonInstance.OnPlayerJoinedDelegate += (player) =>
//            {
//
//                Player remotePlayer = PhotonNetwork.Instantiate("MyPlayer", new Vector3(), new Quaternion())
//                    .GetComponent<Player>();
////                remotePlayer.Name = "Jantje";
//                remotePlayer.Name = "Peter";
//                    Players.Add(remotePlayer);
//            };
        }

        public void NextTurn()
        {
            int index = Array.IndexOf(PhotonNetwork.PlayerList, GetCurrentActivePlayer());
            if (index == -1) return;
            index += 1;
            if (index >= PhotonNetwork.CountOfPlayers)
                index = 0;
//            Player newPlayer = Players[index];
            Photon.Realtime.Player newPlayer = PhotonNetwork.PlayerList[index];
            Hashtable hash = new Hashtable {{"CanMove", true}};
            newPlayer.SetCustomProperties(hash);
            hash = new Hashtable {{"CanMove", false}};
            GetCurrentActivePlayer().SetCustomProperties(hash);
//            GetCurrentActivePlayer().CanMove = false;
//            newPlayer.CanMove = true;
        }

        public Photon.Realtime.Player GetCurrentActivePlayer()
        {
            return PhotonNetwork.PlayerList.FirstOrDefault(x => (bool) x.CustomProperties["CanMove"]);
//            return Players.FirstOrDefault(x => x.CanMove);
        }

        public void ChangeWalkingLetters(char FirstLetter, char SecondLetter)
        {
            foreach (Player player in Players)
            {
                //todo somehow get the walkingletterz of the player
                LetterBlock firstLetter = player.LetterManager.InstantiateLetterButton(FirstLetter, true, false, 0, 0);
                LetterBlock secondLetter = player.LetterManager.InstantiateLetterButton(SecondLetter, false, true, 0, 1);
            }
        }

        private void AddNewPlayer(Photon.Realtime.Player player)
        {
            if (Players != null)
            {
//                Players.Add(player);
            }
        }
    }
}
