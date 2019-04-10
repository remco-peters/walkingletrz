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
        Assert.IsNotNull(MediumGameBoard, "GameBoard misses in GameState");


        PlayerPrefs.DeleteKey("playerWordCount");
        PlayerPrefs.DeleteKey("playerPointsCount");

        // If medium is chosen
        Instantiate(MediumGameBoard);
        Instantiate(CameraClass);
    }
}
