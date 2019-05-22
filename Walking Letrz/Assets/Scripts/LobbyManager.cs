using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject usernamePlaceHolder;
    public GameObject UsernameClass;
    public Button StartGame;

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

        CreatePlayerNameText(player);
    }
    
    void ShowAllPlayers()
    {
        var list = PhotonManager.PhotonInstance.GetOtherPlayersList();
        foreach (var player in list)
        {
            CreatePlayerNameText(player);
        }
    }

    void CreatePlayerNameText(Player player)
    {
        GameObject txtHolder = Instantiate(UsernameClass, usernamePlaceHolder.transform, false);
        txtHolder.GetComponentInChildren<Text>().text = player.NickName;
    }

    void Update()
    {
        if(PhotonManager.PhotonInstance.GetOtherPlayersList().Length > 1)
        {
            if(!StartGame.enabled)
            {
                StartGame.enabled = true;
            }
        } else
        {
            if (StartGame.enabled)
            {
                StartGame.enabled = false;
            }
        }
    }
}
