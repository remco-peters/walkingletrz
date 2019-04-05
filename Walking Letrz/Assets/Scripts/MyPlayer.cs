﻿using System.Collections;
using Assets.Scripts;
using UnityEngine;

public class MyPlayer : MyMonoBehaviour
{
    public float TimeRemaining { get; set; }
    public float CoolDownTime = 10;
    public bool CanMove = true;
    public long EarnedPoints { get; set; }
   // public bool MustThrowLetterAway { get; set; }
    public string InfoText;

    public GameObject WriteBoard, LetterBoardObject;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(WriteBoard);
        //GameObject LetterBoard = Instantiate(LetterBoardObject);
        TimeRemaining = 120;
        EarnedPoints = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining <= 0)
        {
            CanMove = false;
        }
    }

   /* public void StartCooldown()
    {
        StartCoroutine(CoolDownTimer());
    }*/

    IEnumerator CoolDownTimer()
    {
        Debug.Log("Placed a word");
        while(CoolDownTime >= 0)
        {
            CoolDownTime -= Time.deltaTime;
            CanMove = false;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Can place again");

        CanMove = true;
        CoolDownTime = 10;

    }
}
