using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
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
    public Image CrownImage;

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

    public Sprite crownBronze;
    public Sprite crownSilver;
    public Sprite crownGold;

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

    public List<Player> Players
    {
        get
        {
            HUD Hud = GetComponentInParent<HUD>();
            Assert.IsNotNull(Hud, "Hud niet ingesteld in UIPayerPanel");
            Assert.IsNotNull(Hud.PlayersList, "Geen playerslist in hud");

            return Hud.PlayersList;
        }
        set { }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Timer());
        InitOtherPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        string pointString = LocalizationManager.GetTranslation("points");
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

        SetBackgroundPlayerColor();

        int index = 0;
        foreach (Player p in Players)
        {
            if (p != Player)
            {
                SetOpponentPoints(index, p);
                // Change this later on
                if(Player.EarnedPoints > p.EarnedPoints || Player.EarnedPoints == p.EarnedPoints)
                {
                    CrownImage.sprite = crownGold;
                } else
                {
                    CrownImage.sprite = crownSilver;
                }
                index++;
            }
        }
    }

    void InitOtherPlayers()
    {
        OpponentNameTxt.text = "";
        OpponentScoreTxt.text = "";
        OpponentNameTxtSecond.text = "";
        OpponentScoreTxtSecond.text = "";
        OpponentNameTxtThird.text = "";
        OpponentScoreTxtThird.text = "";

        int index = 0;
        foreach(Player p in Players)
        {
            if(p != Player)
            {
                SetOpponentText(index, p);
                index++;
            }
        }
    }

    IEnumerator Timer()
    {
        while (Player.TimeRemaining >= 0)
        {
            // Make the text blinking when waiting for your turn
            if (Player.CanMove == false)
            {
                TimeRemainingText.text = "";
                yield return new WaitForSeconds(0.75f);
                TimeRemainingText.text = TimeText(Player.TimeRemaining);
                yield return new WaitForSeconds(0.75f);
            }

            TimeRemainingText.text = TimeText(Player.TimeRemaining);

            yield return new WaitForEndOfFrame();
        } 

        string timeUp = LocalizationManager.GetTranslation("time_up");
        string playAgain = LocalizationManager.GetTranslation("play_again");
        string pointsPre = LocalizationManager.GetTranslation("points_earned");
        string pointsSuf = LocalizationManager.GetTranslation("points_earned_suffix");
        TimeRemainingText.text = timeUp;
        InfoText.text = $"{timeUp} {playAgain} {pointsPre} {Player.EarnedPoints} {pointsSuf}";
        WrapUpGame();
        InfoText.enabled = true;
        StopCoroutine(Timer());
        gameObject.AddComponent<SceneSwitcher>();
        gameObject.GetComponent<SceneSwitcher>().SwithSceneToMatchResult(Player, Players, "MatchResultScene");
    }

    private string TimeText(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return t.ToString(@"mm\:ss");
    }

    private void SetBackgroundPlayerColor()
    {
        if(Player.CanMove)
        {
            PlayerBackground.GetComponent<Image>().material = TurnMaterial;
            OthersBackground.GetComponent<Image>().material = TopBoardMaterial;
        } else
        {
            PlayerBackground.GetComponent<Image>().material = TopBoardMaterial;
            OthersBackground.GetComponent<Image>().material = TurnMaterial;
        }
    }
    
    public void SetOpponentText(int which, Player p)
    {
        switch(which)
        {
            case 0:
                OpponentNameTxt.text = p.Name;
                OpponentScoreTxt.text = $"{p.EarnedPoints}";
                break;
            case 1:
                OpponentNameTxtSecond.text = p.name;
                OpponentScoreTxtSecond.text = $"{p.EarnedPoints}";
                break;
            case 2:
                OpponentNameTxtThird.text = p.name;
                OpponentScoreTxtThird.text = $"{p.EarnedPoints}";
                break;
            default:
                break;
        }
    }

    public void SetOpponentPoints(int which, Player p)
    {
        switch (which)
        {
            case 0:
                OpponentScoreTxt.text = $"{p.EarnedPoints}";
                break;
            case 1:
                OpponentScoreTxtSecond.text = $"{p.EarnedPoints}";
                break;
            case 2:
                OpponentScoreTxtThird.text = $"{p.EarnedPoints}";
                break;
            default:
                break;
        }
    }

    private void WrapUpGame()
    {
        int creditsToGive = 0;
        Players.Sort((p1, p2) => p2.EarnedPoints.CompareTo(p1.EarnedPoints));
        var indexOfPlayer = Players.IndexOf(Player);
        Debug.Log($"Index of player in list: {indexOfPlayer}");
        if (Players[0].EarnedPoints == Players[1].EarnedPoints)
            creditsToGive = 25;
        else if (indexOfPlayer == 0)
            creditsToGive = 50;
        else
            creditsToGive = 5;
        Debug.Log($"Credits to give: {creditsToGive}");
        Player.Credit.AddCredits(creditsToGive);
    }
}
