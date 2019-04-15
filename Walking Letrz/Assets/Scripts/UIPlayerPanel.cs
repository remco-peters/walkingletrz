using System;
using System.Collections;
using I2.Loc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlayerPanel : UIBehaviour
{
    public Text TimeRemainingText;
    public Text PointText;
    public Text InfoText;
    public Text PlayerNameTxt;
    public Image PlayerImg;

    public Text OpponentNameTxt;
    public Text OpponentScoreTxt;
    public Text OpponentNameTxtSecond;
    public Text OpponentScoreTxtSecond;
    public Text OpponentNameTxtThird;
    public Text OpponentScoreTxtThird;

    public Material TopBoardMaterial;
    public Material TurnMaterial;
    public GameObject PlayerBackground;
    public GameObject OthersBackground;

    public MyPlayer Player
    {
        get
        {
            HUD Hud = GetComponentInParent<HUD>();
            Assert.IsNotNull(Hud, "Hud niet ingesteld in UIPayerPanel");
            Assert.IsNotNull(Hud.Player, "Geen player in hud");

            return Hud.Player;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        
        while (Player.TimeRemaining >= 0)
        {
            TimeRemainingText.text = TimeText(Player.TimeRemaining);

            yield return new WaitForEndOfFrame();
        }

        string timeUp = LocalizationManager.GetTranslation("time_up");
        string playAgain = LocalizationManager.GetTranslation("play_again");
        string pointsPre = LocalizationManager.GetTranslation("points_earned");
        string pointsSuf = LocalizationManager.GetTranslation("points_earned_suffix");
        TimeRemainingText.text = timeUp;
        InfoText.text = $"{timeUp} {playAgain} {pointsPre} {Player.EarnedPoints} {pointsSuf}";
        
        InfoText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        string pointString = I2.Loc.LocalizationManager.GetTranslation("points");
        PointText.text = $"{Player.EarnedPoints}"; // {pointString}

        if(Player.CoolDownTime >= 0 && Player.CoolDownTime < 10)
        {
            InfoText.enabled = true;
            InfoText.text = "Can't move yet: " + TimeText(Player.CoolDownTime) + " seconds remaining";
        } 
        else if(Player.InfoText.Length > 0)
        {
            InfoText.enabled = true;
            InfoText.text = Player.InfoText;
        }else
        {
            InfoText.enabled = false;
        }

        if(Player.CanMove == false)
        {
            StartCoroutine(BlinkingTimerText());
            PlayerBackground.GetComponent<Image>().material = TopBoardMaterial;
            OthersBackground.GetComponent<Image>().material = TurnMaterial;
        } else
        {
            StopCoroutine(BlinkingTimerText());
            PlayerBackground.GetComponent<Image>().material = TurnMaterial;
            OthersBackground.GetComponent<Image>().material = TopBoardMaterial;
        }
    }

    IEnumerator BlinkingTimerText()
    {
        while (Player.CanMove == false)
        {
            TimeRemainingText.text = "";
            yield return new WaitForSeconds(0.75f);
            TimeRemainingText.text = TimeText(Player.TimeRemaining);
            yield return new WaitForSeconds(0.75f);
        }
    }

    private string TimeText(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return t.ToString(@"mm\:ss");
    }

    // Nog bezig met de onderste 2
    public void SetOpponentText(int which, int points, string playerName = "Robot")
    {
        switch(which)
        {
            case 1:
                OpponentNameTxt.text = playerName;
                OpponentScoreTxt.text = $"{points}";
                break;
            case 2:
                OpponentNameTxtSecond.text = playerName;
                OpponentScoreTxtSecond.text = $"{points}";
                break;
            case 3:
                OpponentNameTxtThird.text = playerName;
                OpponentScoreTxtThird.text = $"{points}";
                break;
            default:
                break;
        }
    }

    public void SetOpponentPoints(int which, int points)
    {
        switch (which)
        {
            case 1:
                OpponentScoreTxt.text = $"{points}";
                break;
            case 2:
                OpponentScoreTxtSecond.text = $"{points}";
                break;
            case 3:
                OpponentScoreTxtThird.text = $"{points}";
                break;
            default:
                break;
        }
    }
}
