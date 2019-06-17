using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using Player = Assets.Scripts.Player;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public UnityAction OnJoinedLobbyDelegate;
    public UnityAction OnJoinedRoomDelegate;
    public UnityAction<Photon.Realtime.Player> OnPlayerJoinedDelegate;
    public UnityAction<Photon.Realtime.Player> OnPlayerLeftDelegate;
    
    private UnityAction OnCreatedRoomDelegate;
    private UnityAction<short, string> OnCreateRoomFailedDelegate;

    public static Dictionary<Photon.Realtime.Player, Player> Players { get; } = new Dictionary<Photon.Realtime.Player, Player>();
    public bool createRoom { get; set; }
    public string roomName { get; set; } = "";

    private void Awake()
    {
    }
    /// <summary>
    /// Method to connect with photon. Scenes will be automatically synced, nickname is the display of current player and the userId is PlayFabID
    /// </summary>
    public void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected) return;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LocalPlayer.NickName = AccountManager.CurrentPlayer.DisplayName;
        PhotonNetwork.AuthValues = new AuthenticationValues
        {
            UserId = AccountManager.CurrentPlayerAccount.PlayFabId
        };

        Debug.Log("photon manager awake");
        bool success = PhotonNetwork.ConnectUsingSettings();

        if (!success)
        {
            Debug.LogError("Failed connecting to Photon");
        }

    }
    
    /// <summary>
    /// When connected to master, join lobby
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master.");

        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// When joined lobby, show text in debug
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    /// <summary>
    /// On left lobby, show text in debug
    /// </summary>
    public override void OnLeftLobby()
    {
        Debug.Log("Left lobby");
    }

    /// <summary>
    /// Method that is called when the roomlist is updated. Checks if there was a room with a specific name, otherwhise join existing room, otherwhise create new one
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (PhotonNetwork.InRoom) return;
        
        Debug.Log("Room list updated");

        if (!createRoom)
        {
            foreach (RoomInfo room in roomList)
            {
                if (!string.IsNullOrEmpty(roomName))
                {
                    if (room.Name == roomName)
                    {
                        Debug.Log($"Joining room {room.Name}");

                        if (PhotonNetwork.JoinRoom(room.Name)) return;
                    }
                }
                else
                {
                    if (PhotonNetwork.JoinRandomRoom()) return;
                }
            }
        }
        else
        {
            Debug.Log("Creating room");

            CreateRoom(roomName, () => { Debug.Log("Room created"); },
                (short error, string message) => { Debug.Log($"Room create failed for reason {message}"); });
        }
    }
    
    /// <summary>
    /// Create a new room with 2 players maximum
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Success"></param>
    /// <param name="Failed"></param>
    public void CreateRoom(string Name, UnityAction Success, UnityAction<short, string> Failed)
    {
        OnCreatedRoomDelegate = () => {
            OnCreatedRoomDelegate = null;
            OnCreateRoomFailedDelegate = null;

            Success();
        };

        OnCreateRoomFailedDelegate = (short Error, string Message) => {
            OnCreatedRoomDelegate = null;
            OnCreateRoomFailedDelegate = null;

            Failed(Error, Message);
        };
        
        RoomOptions options = new RoomOptions {MaxPlayers =  2, PublishUserId = true};
        bool Result = PhotonNetwork.CreateRoom(Name, options);

        if (!Result) {
            OnCreatedRoomDelegate = null;
            OnCreateRoomFailedDelegate = null;

            Failed(0x6666, "Create room failed");
        }
    }

    public void Begin()
    {
        Debug.Log("Begin photonmanager");
    }
    /// <summary>
    /// When room is created, delegate will be called
    /// </summary>
    public override void OnCreatedRoom()
    {
        OnCreatedRoomDelegate();
    }

    /// <summary>
    /// When creating room has failed, failedDelegate will be called
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OnCreateRoomFailedDelegate(returnCode, message);
    }

    /// <summary>
    /// When joined room, joinedRoomDelegate will be called
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room {PhotonNetwork.CurrentRoom.Name}");
        
        Player.joinedRoom = true;
        OnJoinedRoomDelegate();
    }
    /// <summary>
    /// On room joined failed, a room will be created to join
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="errorMsg"></param>
    public override void OnJoinRoomFailed(short returnCode, string errorMsg)
    {
        CreateRoom(null, () =>
        {
            Debug.Log("Room created");
        },
             (short error, string message) => { Debug.Log($"Room create failed for reason {message}"); });
    }
    /// <summary>
    /// Method will be called after a player (not the master) has joined the room
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (!newPlayer.IsLocal)
        {
            Debug.Log("3rd partied");
            OnPlayerJoinedDelegate(newPlayer);
        }
    }
    /// <summary>
    /// Method that will be called when player has left the room
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("doei doei");
        OnPlayerLeftDelegate(otherPlayer);
    }

    /// <summary>
    /// Method to leave the lobby
    /// </summary>
    public void LeaveLobby()
    {
        PhotonNetwork.Disconnect();
    }

    #region PhotonManager singleton thingy
    protected static PhotonManager _photonManager;

    public static bool Initialized => _photonManager != null;

    public static PhotonManager PhotonInstance {
        get {
            if (!Initialized) {
                GameObject GameObject = new GameObject("PhotonManager");

                _photonManager = GameObject.AddComponent<PhotonManager>();
                DontDestroyOnLoad(_photonManager);
            }

            return _photonManager;
        }
    }

    public Photon.Realtime.Player[] GetOtherPlayersList()
    {
        return PhotonNetwork.PlayerListOthers;
    }

    public Photon.Realtime.Player[] GetAllPlayersList()
    {
        return PhotonNetwork.PlayerList;
    }
    #endregion
}
