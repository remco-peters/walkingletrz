using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    public LetterManager LetterManagerClass;

    public float TimeRemaining { get; set; }
    public float CoolDownTime = 10;
    public bool CanMove = true;
    public long EarnedPoints { get; set; }

    public GameObject WriteBoard, LetterBoardObject;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(WriteBoard);
        GameObject LetterBoard = Instantiate(LetterBoardObject);
        LetterManager letterManager = Instantiate(LetterManagerClass);
        letterManager.LetterBoard = LetterBoard;
        TimeRemaining = 300;
        letterManager.Player = this;
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

    public void StartCooldown()
    {
        StartCoroutine(CoolDownTimer());
    }

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
