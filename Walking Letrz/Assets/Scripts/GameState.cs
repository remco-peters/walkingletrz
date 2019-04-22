using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState : MyMonoBehaviour
{
    public DynamicUI MediumGameBoard;
    public Camera CameraClass;
    
    public AchievementManager AchievementManagerClass;
    public GameObject gameInstance;
    
    void Awake()
    {
        if (GameInstance.instance == null)
        {
            //Instantiate gameManager prefab
            Instantiate(gameInstance);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(CameraClass, "Camera misses in GameState");
        Assert.IsNotNull(MediumGameBoard, "GameBoard misses in GameState");

        PlayerPrefs.DeleteKey("25PointAchievement");
        PlayerPrefs.DeleteKey("50PointAchievement");
        PlayerPrefs.DeleteKey("100PointAchievement");
        PlayerPrefs.DeleteKey("250PointAchievement");
        PlayerPrefs.DeleteKey("5WordAchievement");
        PlayerPrefs.DeleteKey("10WordAchievement");
        PlayerPrefs.DeleteKey("25WordAchievement");

        // If medium is chosen
        Instantiate(MediumGameBoard);
        Instantiate(CameraClass);
    }
}
