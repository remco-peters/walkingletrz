using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject usernamePlaceHolder;
    public GameObject UsernameClass;
    public Button StartGame;
    public Image LoadingIndicator;
    public GameObject RoomNamePopup;

    private float RotationSpeed = 200f;

    public void SetRoomName(string roomName = "")
    {
        PhotonManager.PhotonInstance.roomName = roomName;
    }

    public void JoinRoom()
    {
        PhotonManager.PhotonInstance.createRoom = false;
        GoConnect();
        Destroy(RoomNamePopup);
        
    }

    public void CreateRoom()
    {
        PhotonManager.PhotonInstance.createRoom = true;
        GoConnect();
        Destroy(RoomNamePopup);
    }

    private void GoConnect()
    {
        PhotonManager.PhotonInstance.ConnectToPhoton();
        PhotonManager.PhotonInstance.OnJoinedRoomDelegate += JoinedRoom;
        PhotonManager.PhotonInstance.OnPlayerJoinedDelegate += PlayerJoined;
        PhotonManager.PhotonInstance.OnPlayerLeftDelegate += PlayerLeft;
    }
    
    private void FixedUpdate()
    {
        LoadingIndicator.GetComponent<RectTransform>().Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }

    void JoinedRoom()
    {
        Hashtable hash = new Hashtable {{"CanMove", false}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        
        ShowAllPlayers();
    }

    void PlayerJoined(Player player)
    {
        Debug.Log(
            $"Player joined: local: {player.IsLocal}, name: {player.NickName}, actornumber: {player.ActorNumber}");

        CreatePlayerNameText(player);
    }

    void PlayerLeft(Player player)
    {
        if (usernamePlaceHolder != null)
        {
            foreach (Transform child in usernamePlaceHolder.transform)
            {
                Destroy(child.gameObject);
            }
            ShowAllPlayers();
        }
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
        if(PhotonManager.PhotonInstance.GetOtherPlayersList().Length > 0 && PhotonNetwork.IsMasterClient)
        {
            if(!StartGame.gameObject.activeInHierarchy)
            {
                StartGame.gameObject.SetActive(true);
            }
        } else
        {
            if (StartGame.gameObject.activeInHierarchy)
            {
                StartGame.gameObject.SetActive(false);
            }
        }
    }
}
