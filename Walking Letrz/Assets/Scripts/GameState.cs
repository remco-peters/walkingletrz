using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState : MyMonoBehaviour
{
    public DynamicUI MediumGameBoard;
    public Camera CameraClass;
    
    public AchievementManager AchievementManagerClass;

    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(CameraClass, "Camera misses in GameState");
        //AchievementManager achievementManager = Instantiate(AchievementManagerClass);
        //MyPlayer player = Spawn(PlayerClass, this, p => { p.AchievementManager = achievementManager; });
        
        //PlayerManagerClass.Players = new List<Player> {player, BotClass}; //todo add bots or other players
        
        PlayerPrefs.DeleteKey("playerWordCount");
        PlayerPrefs.DeleteKey("playerPointsCount");
        Assert.IsNotNull(MediumGameBoard, "GameBoard misses in GameState");
        
        // If medium is chosen
        Instantiate(MediumGameBoard);
        Instantiate(CameraClass);
    }
}
