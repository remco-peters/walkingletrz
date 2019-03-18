using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    public LetterManager LetterManagerClass;

    public float TimeRemaining {get; set; }
    private float CoolDownTime = 10;
    private bool CanMove = true;

    public GameObject WriteBoard, LetterBoardObject;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(WriteBoard);
        GameObject LetterBoard = Instantiate(LetterBoardObject);
        LetterManager letterManager = Instantiate(LetterManagerClass);
        letterManager.LetterBoard = LetterBoard;
        TimeRemaining = 300;
    }

    // Update is called once per frame
    void Update()
    {
        TimeRemaining -= Time.deltaTime;

        if (CanMove && Input.GetKeyDown(KeyCode.Space)) // If player sents a word
        {
            CanMove = false;
            Debug.Log("Clicked!");
        }

        if (CanMove == false)
        {
            CoolDownTime -= Time.deltaTime;
        }

        if (CoolDownTime <= 0 && TimeRemaining > 0)
        {
            CanMove = true;
            Debug.Log("You can click again!");
            CoolDownTime = 10;
        }

        if(TimeRemaining <= 0)
        {
            CanMove = false;
        }

    }
}
