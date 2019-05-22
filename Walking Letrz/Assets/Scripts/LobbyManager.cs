using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonManager.PhotonInstance.OnJoinedRoomDelegate = JoinedRoom;
        PhotonManager.PhotonInstance.OnPlayerJoinedDelegate += PlayerJoined;
    }

    void JoinedRoom()
    {
        
    }

    void PlayerJoined(Player player)
    {
        Debug.Log(
            $"Player joined: local: {player.IsLocal}, name: {player.NickName}, actornumber: {player.ActorNumber}");
    }
}
