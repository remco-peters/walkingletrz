using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject usernamePlaceHolder;
    public Text UsernameClass;

    // Start is called before the first frame update
    void Start()
    {
        PhotonManager.PhotonInstance.OnJoinedRoomDelegate += JoinedRoom;
        PhotonManager.PhotonInstance.OnPlayerJoinedDelegate += PlayerJoined;
    }

    void JoinedRoom()
    {
        ShowAllPlayers();
    }

    void PlayerJoined(Player player)
    {
        Debug.Log(
            $"Player joined: local: {player.IsLocal}, name: {player.NickName}, actornumber: {player.ActorNumber}");

        Text txt = Instantiate(UsernameClass);
        txt.text = player.NickName;
        txt.transform.SetParent(usernamePlaceHolder.transform, false);
    }
    
    void ShowAllPlayers()
    {
        var list = PhotonManager.PhotonInstance.GetOtherPlayersList();
        foreach (var player in list)
        {
            Text txt = Instantiate(UsernameClass);
            txt.text = player.NickName;
            txt.transform.SetParent(usernamePlaceHolder.transform, false);
        }
    }
}
