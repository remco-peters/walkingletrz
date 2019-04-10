using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState : MyMonoBehaviour
{
    public DynamicUI MediumGameBoard;
    public Camera CameraClass;
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(CameraClass, "Camera misses in GameState");
        Assert.IsNotNull(MediumGameBoard, "GameBoard misses in GameState");
        
        // If medium is chosen
        Instantiate(MediumGameBoard);
        Instantiate(CameraClass);
    }
}
