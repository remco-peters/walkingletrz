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

    /// <summary>
    /// If a room name is entered it is set in the photonmanager
    /// </summary>
    /// <param name="roomName"></param>
    public void SetRoomName(string roomName = "")
    {
        PhotonManager.PhotonInstance.roomName = roomName;
    }

    /// <summary>
    /// Join a room, so a room doesn't have to be created
    /// destroys the room name popup
    /// </summary>
    public void JoinRoom()
    {
        PhotonManager.PhotonInstance.createRoom = false;
        GoConnect();
        Destroy(RoomNamePopup);
        
    }

    /// <summary>
    /// Create a room, so createRoom = true
    /// destroys the room name popup
    /// </summary>
    public void CreateRoom()
    {
        PhotonManager.PhotonInstance.createRoom = true;
        GoConnect();
        Destroy(RoomNamePopup);
    }

    /// <summary>
    /// Connect to photon and set all delegates
    /// </summary>
    private void GoConnect()
    {
        PhotonManager.PhotonInstance.ConnectToPhoton();
        PhotonManager.PhotonInstance.OnJoinedRoomDelegate += JoinedRoom;
        PhotonManager.PhotonInstance.OnPlayerJoinedDelegate += PlayerJoined;
        PhotonManager.PhotonInstance.OnPlayerLeftDelegate += PlayerLeft;
    }
    
    /// <summary>
    /// Rotates the loading indicator at a constant pace
    /// </summary>
    private void FixedUpdate()
    {
        LoadingIndicator.GetComponent<RectTransform>().Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// When joining a room, set CanMove to false (you are the 2nd player)
    /// Tell ui that a player has joined and needs to update
    /// </summary>
    void JoinedRoom()
    {
        Hashtable hash = new Hashtable {{"CanMove", false}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        
        ShowAllPlayers();
    }

    /// <summary>
    /// When a player joins the room tell ui to update
    /// </summary>
    /// <param name="player"></param>
    void PlayerJoined(Player player)
    {
        Debug.Log(
            $"Player joined: local: {player.IsLocal}, name: {player.NickName}, actornumber: {player.ActorNumber}");

        CreatePlayerNameText(player);
    }

    /// <summary>
    /// Method that will be called when a user has left the lobby
    /// </summary>
    /// <param name="player"></param>
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
    
    /// <summary>
    /// Method to show all the players in current room
    /// </summary>
    void ShowAllPlayers()
    {
        var list = PhotonManager.PhotonInstance.GetOtherPlayersList();
        foreach (var player in list)
        {
            CreatePlayerNameText(player);
        }
    }
    /// <summary>
    /// Creates the object with the name of the player and places this
    /// </summary>
    /// <param name="player"></param>
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
