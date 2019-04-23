using System.Collections;
using Assets.Scripts;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : Player
{
    public float CoolDownTime = 10;
    public string InfoText;
    public GameObject WriteBoard, LetterBoardObject;
    public AchievementManager AchievementManager { private get; set; }
    public Credit Credit { get; set; }

    private int placedWordCount;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        AchievementManager.Player = this;
        CanMove = true;
        placedWordCount = 0;
        Instantiate(Credit);
        //Todo fix this
        //Instantiate(WriteBoard);
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
        while (CoolDownTime >= 0)
        {
            CoolDownTime -= Time.deltaTime;
            CanMove = false;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Can place again");

        CanMove = true;
        CoolDownTime = 10;
    }

    public void IncreaseWordCount()
    {
        AchievementManager.SubmitWordCountToAchievements(++placedWordCount);
        AchievementManager.SubmitPointsToAchievements(EarnedPoints);
        InfoText = AchievementManager.CheckIfAchievementIsGet();
        Text infoTxt = GameObject.FindGameObjectWithTag("AchievementUnlockTxt").GetComponent<Text>();
        infoTxt.text = InfoText;
        GameObject.FindGameObjectWithTag("AchievementUnlockPanel").GetComponent<ShowInfoText>().ShowToastPanel(2);
        infoTxt.GetComponent<ShowInfoText>().ShowToast(2);
    }

    public int GetPlacedWordCount()
    {
        return placedWordCount;
    }
}
