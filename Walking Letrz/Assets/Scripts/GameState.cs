using System.Collections.Generic;
using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class GameState : MonoBehaviourPunCallbacks
{
    public DynamicUI DynamicGameBoard;
    public Camera CameraClass;
    public AchievementManager AchievementManagerClass;
    private bool Tutorial;
    
    public UnityAction OnJoinedLobbyDelegate;
    public UnityAction OnJoinedRoomDelegate;
    
    private UnityAction OnCreatedRoomDelegate;
    private UnityAction<short, string> OnCreateRoomFailedDelegate;

    public static List<string> PlacedWordsInThisGame = new List<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(CameraClass, "Camera misses in GameState");
        Assert.IsNotNull(DynamicGameBoard, "GameBoard misses in GameState");
        
        // If medium is chosen
        DynamicUI GBoard = Instantiate(DynamicGameBoard);
        GBoard.Tutorial = GetTutorial();
        Instantiate(CameraClass);
    }
    
    private bool GetTutorial()
    {
        if (GameInstance.instance.IsMultiplayer) return false;

        if (PlayerPrefs.GetInt("HadTutorialGame") == 0)
        {
            PlayerPrefs.SetInt("HadTutorialGame", 1);
            return true;
        } else
        {
            return false;
        }
    }
}
