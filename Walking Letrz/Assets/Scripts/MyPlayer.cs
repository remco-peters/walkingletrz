using System.Collections;
using Assets.Scripts;
using UnityEngine;

public class MyPlayer : Player
{
    public float CoolDownTime = 10;
    public string InfoText;
    public GameObject WriteBoard, LetterBoardObject;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        CanMove = true;
        Instantiate(WriteBoard);
        //GameObject LetterBoard = Instantiate(LetterBoardObject);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
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
