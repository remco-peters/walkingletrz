using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
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

    private void Awake()
    {
    }

    public void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected) return;
        PhotonNetwork.LocalPlayer.NickName = AccountManager.CurrentPlayer.DisplayName;
        PhotonPeer.RegisterType(typeof(LetterPosition), (byte) 'L', LetterPosition.Serialize,
            LetterPosition.Deserialize);
        
        
        Debug.Log("photon manager awake");
        bool success = PhotonNetwork.ConnectUsingSettings();

        if (!success)
        {
            Debug.LogError("Failed connecting to Photon");
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 1;
        PhotonNetwork.SerializationRate = 1;
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master.");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left lobby");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (PhotonNetwork.InRoom) return;
        
        Debug.Log("Room list updated");

        foreach (RoomInfo room in roomList)
        {
            if (room.IsOpen)
            {
                Debug.Log($"Joining room {room.Name}");

                if (PhotonNetwork.JoinRoom(room.Name)) return;
            }
        }
        
        Debug.Log("Creating room");

        CreateRoom(null, () =>
            {
                Debug.Log("Room created");
            },
            (short error, string message) => { Debug.Log($"Room create failed for reason {message}"); });
    }
    
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
        RoomOptions options = new RoomOptions {MaxPlayers = 2};
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

    public override void OnCreatedRoom()
    {
        OnCreatedRoomDelegate();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OnCreateRoomFailedDelegate(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        
        Player.joinedRoom = true;
        OnJoinedRoomDelegate();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (!newPlayer.IsLocal)
        {
//            newPlayer.NickName = AccountManager.CurrentPlayer.DisplayName;
            Debug.Log("3rd partied");
            OnPlayerJoinedDelegate(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("doei doei");
        OnPlayerLeftDelegate(otherPlayer);
    }

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
    #endregion
}
