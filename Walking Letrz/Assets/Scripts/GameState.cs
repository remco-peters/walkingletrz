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
    public Text TimerText;
    public HUD HUDClass;

    private float TimeRemaining = 300;
    private float CoolDown = 10;
    private bool CanMove = true;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(CameraClass, "Camera misses in GameState");
        Assert.IsNotNull(PlayerClass, "Player misses in GameState");
        Assert.IsNotNull(HUDClass, "HUD misses in GameState");

        Instantiate(CameraClass);
        Instantiate(PlayerClass);
        TimerText.text = TimeText(TimeRemaining);
        Instantiate(TimerText);
    }

    // Update is called once per frame
    void Update()
    {
        TimeRemaining -= Time.deltaTime;
        TimerText.text = TimeText(TimeRemaining);

        if(TimeRemaining <= 0)
        {
            TimerText.text = "Time's up!";
            // Time's up - Next scene...
        } else
        {
            if (CanMove && Input.GetMouseButtonDown(0)) // If player sents a word
            {
                CanMove = false;
                Debug.Log("Clicked!");
            }

            if(CanMove == false)
            {
                CoolDown -= Time.deltaTime;
            }

            if(CoolDown <= 0)
            {
                CanMove = true;
                Debug.Log("You can click again!");
                CoolDown = 10;
            }
        }
    }

    private string TimeText(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return t.ToString(@"mm\:ss");
    }
}
