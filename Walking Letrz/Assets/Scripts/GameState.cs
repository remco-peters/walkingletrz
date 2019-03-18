using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public Camera CameraClass;
    public MyPlayer PlayerClass;
    public HUD HUDClass;
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(CameraClass, "Camera misses in GameState");
        Assert.IsNotNull(PlayerClass, "Player misses in GameState");
        Assert.IsNotNull(HUDClass, "HUD misses in GameState");

        Instantiate(CameraClass);
        MyPlayer Player = Instantiate(PlayerClass);
        HUD HUD = Instantiate(HUDClass);
        HUD.Player = Player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
